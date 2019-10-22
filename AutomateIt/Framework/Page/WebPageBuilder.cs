using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using automateit.SCSS;
using Natu.Utils.Extensions;
using NUnit.Framework;
using selenium.core.Framework.PageElements;

namespace selenium.core.Framework.Page
{
    public static class WebPageBuilder
    {
        public static void InitPage(IPage page) {
            InitWatch();
            InitComponents(page, page);
            ReportWatch(page.GetType());
        }

        public static List<T> CreateItems<T>(IContainer container, int itemsCount)
            where T : IItem {
            var itemIds = new List<string>();
            for (var i = 1; i <= itemsCount; i++) {
                itemIds.Add(i.ToString());
            }
            return CreateItems<T>(container, itemIds);
        }
        
        /// <summary>
        ///     Создать список элементов и для каждого элемента инициализировать все вложенные компоненты
        ///     (помеченные атрибутом WebComponent)
        /// </summary>
        public static List<T> CreateItems<T>(IContainer container, IEnumerable<string> ids) where T : IItem {
            var items = new ConcurrentBag<T>();
            Parallel.ForEach(ids, id =>
                {
                    items.Add(CreateComponent<T>(container.ParentPage, container, id));
                });
            return items.ToList();
        }

        public static List<T> CreateDummyItems<T>(IContainer container, IEnumerable<string> ids) where T : IItem {
            var items = new ConcurrentBag<T>();
            Parallel.ForEach(ids, id =>
                {
                    var args = new List<object> { container, id };
                    items.Add((T)Activator.CreateInstance(typeof(T), args.ToArray()));
                });
            return items.ToList();
        }

        public static T CreateComponent<T>(IContainer container, params object[] additionalArgs)
        {
            InitWatch();
            var component = CreateComponent(container.ParentPage, container, typeof(T),
                new WebComponentAttribute(additionalArgs),null);
            InitComponents(container.ParentPage, component);
            ReportWatch(typeof(T));
            return (T)component;
        }

        public static IComponent CreateComponent<T>(IPage page, params object[] additionalArgs)
        {
            InitWatch();
            var type = typeof(T);
            var component = CreateComponent(page, page, type, new WebComponentAttribute(additionalArgs),null);
            InitComponents(page, component);
            ReportWatch(type);
            return component;
        }

        /// <summary>
        ///     Создать компонент и инициализировать все вложенные компоненты
        ///     (помеченные атрибутом WebComponent)
        /// </summary>
        public static T CreateComponent<T>(IPage page, object componentContainer, params object[] additionalArgs)
        {
            InitWatch();
            var type = typeof(T);
            var component = CreateComponent(page, componentContainer, type,
                new WebComponentAttribute(additionalArgs),null);
            InitComponents(page, component);
            ReportWatch(type);
            return (T)component;
        }

        public static IComponent CreateComponent(IPage page, Type type, params object[] additionalArgs) {
            InitWatch();
            var component = CreateComponent(page, page, type, new WebComponentAttribute(additionalArgs),null);
            InitComponents(page, component);
            ReportWatch(type);
            return component;
        }

        private static readonly bool _report=false;
        private static readonly object _lockConsole = new object();
        private static readonly ConcurrentDictionary<int,Stopwatch> _createInstanceStopwatch = new ConcurrentDictionary<int, Stopwatch>();
        private static readonly ConcurrentDictionary<int,Stopwatch> _getAttributesStopwatch = new ConcurrentDictionary<int, Stopwatch>();
        private static readonly ConcurrentDictionary<int,Stopwatch> _totalStopwatch = new ConcurrentDictionary<int, Stopwatch>();

        private static void InitWatch() {
            _createInstanceStopwatch.AddOrUpdate(Thread.CurrentThread.ManagedThreadId, new Stopwatch(),(key,oldv)=>new Stopwatch());
            _getAttributesStopwatch.AddOrUpdate(Thread.CurrentThread.ManagedThreadId, new Stopwatch(), (key, oldv) => new Stopwatch());
            _totalStopwatch.AddOrUpdate(Thread.CurrentThread.ManagedThreadId,new Stopwatch(), (key, oldv) => new Stopwatch());
            _totalStopwatch[Thread.CurrentThread.ManagedThreadId].Start();
        }

        private static void ReportWatch(Type type) {
            if (_report) {
                _totalStopwatch[Thread.CurrentThread.ManagedThreadId].Stop();
                if (_totalStopwatch[Thread.CurrentThread.ManagedThreadId].ElapsedMilliseconds > 0) {
                    lock (_lockConsole) {
                        Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId} - {Thread.CurrentThread.Priority}");
                        Console.WriteLine($"{type.Name.ToUpper()} init statistic:");
                        Console.WriteLine($"    Create instance: {_createInstanceStopwatch[Thread.CurrentThread.ManagedThreadId].ElapsedMilliseconds} ms.");
                        Console.WriteLine($"    Get attributes instance: {_getAttributesStopwatch[Thread.CurrentThread.ManagedThreadId].ElapsedMilliseconds} ms.");
                        Console.WriteLine($"    Total: {_totalStopwatch[Thread.CurrentThread.ManagedThreadId].ElapsedMilliseconds} ms. ({_createInstanceStopwatch[Thread.CurrentThread.ManagedThreadId].ElapsedMilliseconds + _getAttributesStopwatch[Thread.CurrentThread.ManagedThreadId].ElapsedMilliseconds} ms.)");
                    }
                }
            }
        }

        public static IComponent CreateComponent(
            IPage page, object componentContainer, Type type,
            IComponentAttribute attribute, string componentFieldName)
        {
            var args = typeof(ItemBase).IsAssignableFrom(type)
                ? new List<object> { componentContainer } // костыль
                : new List<object> { page };
            var container = componentContainer as IContainer;
            if (attribute.Args != null)
            {
                // Преобразовать относительные пути в абсолютные
                for (var i = 0; i < attribute.Args.Length; i++)
                {
                    args.Add(CreateInnerSelector(container, attribute.Args[i]));
                }
            }

            var componentName = GetComponentName(attribute.ComponentName, componentFieldName, type.Name);
            if (componentContainer is IItem)
            {
                var itemId = (componentContainer as IItem).ID;
                componentName = $"{componentName} ({itemId})";
            }
            IComponent component;
            try
            {
                _createInstanceStopwatch[Thread.CurrentThread.ManagedThreadId].Start();
                component = (IComponent)Activator.CreateInstance(type, args.ToArray());
                _createInstanceStopwatch[Thread.CurrentThread.ManagedThreadId].Stop();
            }
            catch (MissingMemberException)
            {
                Console.WriteLine($"Can not create instance of component '{componentName}' in '{componentContainer.GetType().Name}'.");
                throw;
            }
            component.ComponentName = componentName;
            component.FrameScss = attribute.FrameScss ?? container?.FrameScss;
            var hasDefaultAction = component as IHasDefaultAction;
            if (hasDefaultAction != null)
            {
                hasDefaultAction.DefaultActionWaitCondition = attribute.DefaultActionWaitCondition;
                hasDefaultAction.DefaultActionWaitTimeout = attribute.DefaultActionWaitTimeout;
            }
            return component;
        }

        private static string GetComponentName(string attributeComponentName, string componentFieldName, string componentTypeName) {
            if (!string.IsNullOrWhiteSpace(attributeComponentName)) {
                return attributeComponentName;
            }
            if (!string.IsNullOrWhiteSpace(componentFieldName)) {
                return componentFieldName.AddSpaces();
            }
            return componentTypeName;
        }

        private static object CreateInnerSelector(IContainer container, object argument) {
            var argumentString = argument as string;
            if (argumentString != null
                && argumentString.StartsWith("root:", StringComparison.Ordinal)) {
                return container.InnerScss(argumentString.Replace("root:", string.Empty));
            }
            return argument;
        }

        /// <summary>
        ///     Инициализировать компоненты
        /// </summary>
        /// <remarks>
        ///     Через Reflection найти и инициализировать все поля объекта реализующие интерфейс IComponent
        /// </remarks>
        public static void InitComponents(IPage page, object containerObject)
        {
            if (page == null)
                throw new ArgumentNullException("page", "page cannot be null");
            if (containerObject == null)
                containerObject = page;
            var container = containerObject as IContainer;
            var type = containerObject.GetType();
            var components = GetComponents(type);
            foreach (var memberInfo in components.Keys)
            {
                var attribute = components[memberInfo];
                IComponent instance;
                if (memberInfo is FieldInfo)
                {
                    var fieldInfo = memberInfo as FieldInfo;
                    instance = (IComponent)fieldInfo.GetValue(containerObject);
                    if (instance == null)
                    {
                        instance = CreateComponent(page, containerObject, fieldInfo.FieldType, attribute, fieldInfo.Name);
                        fieldInfo.SetValue(containerObject, instance);
                    }
                    else
                    {
                        instance.FrameScss = instance.FrameScss ??(attribute.FrameScss??container?.FrameScss);
                        instance.ComponentName = attribute.ComponentName ?? instance.ComponentName;
                    }
                }
                else if (memberInfo is PropertyInfo)
                {
                    var propertyInfo = memberInfo as PropertyInfo;
                    instance = (IComponent)propertyInfo.GetValue(containerObject);
                    if (instance == null)
                    {
                        instance = CreateComponent(page, containerObject, propertyInfo.PropertyType, attribute,propertyInfo.Name);
                        propertyInfo.SetValue(containerObject, instance);
                    }
                    else
                    {
						instance.FrameScss = instance.FrameScss ?? (attribute.FrameScss ?? container?.FrameScss);
                        instance.ComponentName = attribute.ComponentName ?? instance.ComponentName;
                    }
                }
                else
                    throw new NotSupportedException("Unknown member type");
                page.RegisterComponent(instance);
                InitComponents(page, instance);
            }
        }

        public static readonly ConcurrentDictionary<Type,Dictionary<MemberInfo, IComponentAttribute>> TypeComponentsCash = new ConcurrentDictionary<Type, Dictionary<MemberInfo, IComponentAttribute>>();
        public static bool EnableComponentsCashing=true;

        /// <summary>
        ///     Получить список полей-компонентов типа(включая поля-компоненты родительских типов)
        /// </summary>
        private static Dictionary<MemberInfo, IComponentAttribute> GetComponents(Type type) {
            _getAttributesStopwatch[Thread.CurrentThread.ManagedThreadId].Start();
            Dictionary<MemberInfo, IComponentAttribute> components;
            if (!EnableComponentsCashing
                || !TypeComponentsCash.ContainsKey(type)) {
                components = new Dictionary<MemberInfo, IComponentAttribute>();
                // Получить список полей
                var members =
                    type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                        .Cast<MemberInfo>().ToList();
                // Получить список свойств
                members.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.Instance));
                var attributeType = typeof(IComponentAttribute);
                foreach (var field in members) {
                    var attributes = field.GetCustomAttributes(attributeType, true);
                    if (attributes.Length > 0) {
                        if (!IsComponent(field)) {
                            throw new Exception("IComponentAttribute attribute can be applied only to IComponent field");
                        }
                        components.Add(field, attributes[0] as IComponentAttribute);
                    }
                }
                TypeComponentsCash[type] = components;
            }
            else {
                components = TypeComponentsCash[type];
            }
            _getAttributesStopwatch[Thread.CurrentThread.ManagedThreadId].Stop();
            return components;
        }

        private static bool IsComponent(MemberInfo memberInfo)
        {
            var componentType = typeof(IComponent);
            if (memberInfo is FieldInfo)
            {
                return componentType.IsAssignableFrom(((FieldInfo)memberInfo).FieldType);
            }
            if (memberInfo is PropertyInfo)
            {
                return componentType.IsAssignableFrom(((PropertyInfo)memberInfo).PropertyType);
            }
            return false;
        }
    }

    [TestFixture]
    public class WebPageBuilderTest
    {
        [Test]
        public void DoNotAddRootWithouPrefix()
        {
            var page = new Page();
            var container = new Container(page, "//*[@id='rootelementid']");
            WebPageBuilder.InitComponents(page, container);
            Assert.AreEqual("//div[text()" +
                            "='mytext']",
                container.Component2.Xpath, "Относительный xpath не преобразовался в абсолютный");
        }

        [Test]
        public void ReplacePrefixWithRootSelector()
        {
            var page = new Page();
            var container = new Container(page, "//*[@id='rootelementid']");
            WebPageBuilder.InitComponents(page, container);
            Assert.AreEqual("//*[@id='rootelementid']/descendant::div[text()='mytext']",
                container.Component1.Xpath, "Относительный xpath не преобразовался в абсолютный");
        }

        private class Container : ContainerBase
        {
            [WebComponent("root:div[text()='mytext']")]
            public Component Component1;

            [WebComponent("//div[text()='mytext']")]
            public Component Component2;

            public Container(IPage parent, string rootScss)
                : base(parent, rootScss)
            {
            }
        }

        private class Component : ComponentBase
        {
            public readonly string Xpath;

            public Component(IPage page, string xpath)
                : base(page)
            {
                Xpath = xpath;
            }

            public override Selector Selector { get; }

            public override bool IsVisible()
            {
                throw new NotImplementedException();
            }

            public override bool IsExist()
            {
                throw new NotImplementedException();
            }

            public override bool IsNotVisible()
            {
                throw new NotImplementedException();
            }

            public override bool HasClass(string className)
            {
                throw new NotImplementedException();
            }

            public override bool IsDisabled() {
                throw new NotImplementedException();
            }

            public override T GetValue<T>() {
                throw new NotImplementedException();
            }

            public override void Click(int sleepTimeout = 0) {
                throw new NotImplementedException();
            }
        }

        private class Page : PageBase
        {
        }
    }
}
