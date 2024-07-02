import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DataCrawlComponent } from './data-crawl.component';

describe('DataCrawlComponent', () => {
  let component: DataCrawlComponent;
  let fixture: ComponentFixture<DataCrawlComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DataCrawlComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DataCrawlComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
