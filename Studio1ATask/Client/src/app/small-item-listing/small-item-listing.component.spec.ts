import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {SmallItemListingComponent} from './small-item-listing.component';

describe('SmallItemListingComponent', () => {
  let component: SmallItemListingComponent;
  let fixture: ComponentFixture<SmallItemListingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SmallItemListingComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmallItemListingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
