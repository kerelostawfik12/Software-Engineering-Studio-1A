import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewSellersComponent } from './view-sellers.component';

describe('ViewSellersComponent', () => {
  let component: ViewSellersComponent;
  let fixture: ComponentFixture<ViewSellersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewSellersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewSellersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
