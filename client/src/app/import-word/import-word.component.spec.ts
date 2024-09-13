/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { ImportWordComponent } from './import-word.component';

describe('ImportWordComponent', () => {
  let component: ImportWordComponent;
  let fixture: ComponentFixture<ImportWordComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ImportWordComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportWordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
