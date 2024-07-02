import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PermisionComponent } from './permision.component';

describe('PermisionComponent', () => {
  let component: PermisionComponent;
  let fixture: ComponentFixture<PermisionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PermisionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PermisionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
