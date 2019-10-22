"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var testing_1 = require("@angular/core/testing");
var forms_1 = require("@angular/forms");
var router_1 = require("@angular/router");
var testing_2 = require("@angular/router/testing");
var add_category_component_1 = require("../../app/add-category/add-category.component");
var catservice_service_1 = require("../../app/services/catservice.service");
var forms_2 = require("@angular/forms");
var rxjs_1 = require("rxjs");
var http_1 = require("@angular/common/http");
var core_2 = require("@angular/core");
exports.BASE_URL = new core_2.InjectionToken('BASE_URL');
describe('Component: Add Category (Create)', function () {
    var fixture;
    var component;
    var categoryService;
    var router;
    var initialCategory;
    beforeEach(testing_1.async(function () {
        categoryService = jasmine.createSpyObj('categoryService', ['saveCategory']);
        categoryService.saveCategory.and.returnValue(rxjs_1.Observable.of(''));
        testing_1.TestBed.configureTestingModule({
            imports: [
                testing_2.RouterTestingModule,
                forms_2.ReactiveFormsModule,
                http_1.HttpClientModule
            ],
            declarations: [add_category_component_1.AddCategoryComponent],
            providers: [
                forms_1.FormBuilder
            ],
            schemas: [core_1.NO_ERRORS_SCHEMA]
        }).overrideComponent(add_category_component_1.AddCategoryComponent, {
            set: {
                providers: [
                    {
                        provide: catservice_service_1.CategoryService, useValue: categoryService
                    }
                ]
            }
        }).compileComponents()
            .then(function () {
            fixture = testing_1.TestBed.createComponent(add_category_component_1.AddCategoryComponent);
            component = fixture.componentInstance;
            //categoryService = TestBed.get(CategoryService);
            //saveCategorySpy = spyOn(categoryService, 'saveCategory');
            router = testing_1.TestBed.get(router_1.Router);
            spyOn(router, 'navigate');
        });
        initialCategory = {
            categoryId: '00000000-0000-0000-0000-000000000000',
            categoryName: ''
        };
    }));
    function setValidCategory() {
        initialCategory.categoryName = 'MY TEST CATEGORY';
        component.categoryForm.controls['categoryName'].setValue(initialCategory.categoryName);
        return initialCategory;
    }
    function setInvalidCategory() {
        // .do nothing since initial category in creation mode is invalid
        return initialCategory;
    }
    it('should not have error message after init', function () {
        expect(component.errorMessage).toBeFalsy();
    });
    it('should be initialized in creation mode', function () {
        expect(component.title).toEqual('Create');
        expect(component.categoryId).toBeFalsy();
        expect(component.categoryForm).toBeTruthy();
        expect(component.categoryForm.controls['categoryId'].value).toEqual(initialCategory.categoryId);
        expect(component.categoryForm.controls['categoryName'].value).toEqual(initialCategory.categoryName);
    });
    it('submit should not trigger saving category when form is invalid', function () {
        // .Arrange
        setInvalidCategory();
        // .Act
        component.save();
        // .Assert
        expect(categoryService.saveCategory).not.toHaveBeenCalled();
    });
    it('submit should trigger saving category when form is valid', function () {
        // .Arrange
        var validCategory = setValidCategory();
        // .Act
        component.save();
        // .Assert
        expect(categoryService.saveCategory).toHaveBeenCalledWith(validCategory);
    });
    it('should redirect to categories list after successfull submit', function () {
        // .Arrange
        setValidCategory();
        // .Act
        component.save();
        // .Assert
        expect(router.navigate).toHaveBeenCalledWith(['/fetch-category']);
    });
    it('should set error message after submit error', function () {
        // .Arrange
        var expectedError = "MY TEST ERROR";
        categoryService.saveCategory.and.returnValue(rxjs_1.Observable.throw(expectedError));
        setValidCategory();
        // .Act
        component.save();
        // .Assert
        expect(component.errorMessage).toEqual(expectedError);
    });
    it('should redirect to categories list after cancel', function () {
        // .Act
        component.cancel();
        // .Assert
        expect(router.navigate).toHaveBeenCalledWith(['/fetch-category']);
    });
});
//# sourceMappingURL=add-category.component.spec.js.map