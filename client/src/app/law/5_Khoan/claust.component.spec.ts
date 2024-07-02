import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ClaustComponent } from './claust.component';

describe('ClaustComponent', () => {
  let component: ClaustComponent;
  let fixture: ComponentFixture<ClaustComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ClaustComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ClaustComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
