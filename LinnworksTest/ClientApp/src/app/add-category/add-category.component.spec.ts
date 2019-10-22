import { NO_ERRORS_SCHEMA } from '@angular/core';
import {
  TestBed,
  async,
  ComponentFixture
} from '@angular/core/testing';
import { FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';

import { AddCategoryComponent } from '../../app/add-category/add-category.component';
import { CategoryService } from '../../app/services/catservice.service'
import { ReactiveFormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import { HttpClientModule } from '@angular/common/http';
import { InjectionToken } from '@angular/core';
export const BASE_URL = new InjectionToken<string>('BASE_URL');

describe('Component: Add Category (Create)', () => {
  let fixture: ComponentFixture<AddCategoryComponent>;
  let component: AddCategoryComponent;
  let categoryService;
  let router;
  let initialCategory;

  beforeEach(async(() => {
    categoryService = jasmine.createSpyObj('categoryService', ['saveCategory']);
    categoryService.saveCategory.and.returnValue(Observable.of(''));
      
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        ReactiveFormsModule,
        HttpClientModule],
      declarations: [AddCategoryComponent],
      providers: [
        FormBuilder
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).overrideComponent(AddCategoryComponent, {
      set: {
        providers: [
          {
            provide: CategoryService, useValue: categoryService
          }
        ]
      }
    }).compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(AddCategoryComponent);
        component = fixture.componentInstance;
        //categoryService = TestBed.get(CategoryService);
        //saveCategorySpy = spyOn(categoryService, 'saveCategory');
        router = TestBed.get(Router);
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

  it('should not have error message after init', () => {
    expect(component.errorMessage).toBeFalsy();
  });

  it('should be initialized in creation mode', () => {
    expect(component.title).toEqual('Create');
    expect(component.categoryId).toBeFalsy();
    expect(component.categoryForm).toBeTruthy();
    expect(component.categoryForm.controls['categoryId'].value).toEqual(initialCategory.categoryId);
    expect(component.categoryForm.controls['categoryName'].value).toEqual(initialCategory.categoryName);
  });

  it('submit should not trigger saving category when form is invalid', () => {
    // .Arrange
    setInvalidCategory();
    // .Act
    component.save();
    // .Assert
    expect(categoryService.saveCategory).not.toHaveBeenCalled();
  });

  it('submit should trigger saving category when form is valid', () => {
    // .Arrange
    let validCategory = setValidCategory();
    // .Act
    component.save();
    // .Assert
    expect(categoryService.saveCategory).toHaveBeenCalledWith(validCategory);
  });

  it('should redirect to categories list after successfull submit', () => {
    // .Arrange
    setValidCategory();
    // .Act
    component.save();
    // .Assert
    expect(router.navigate).toHaveBeenCalledWith(['/fetch-category']);
  });

  it('should set error message after submit error', () => {
    // .Arrange
    const expectedError = "MY TEST ERROR";
    categoryService.saveCategory.and.returnValue(Observable.throw(expectedError));
    setValidCategory();
    // .Act
    component.save();
    // .Assert
    expect(component.errorMessage).toEqual(expectedError);
  });

  it('should redirect to categories list after cancel', () => {
    // .Act
    component.cancel();
    // .Assert
    expect(router.navigate).toHaveBeenCalledWith(['/fetch-category']);
  });

});
