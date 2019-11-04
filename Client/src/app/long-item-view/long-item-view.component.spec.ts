import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LongItemViewComponent } from './long-item-view.component';

describe('LongItemViewComponent', () => {
  let component: LongItemViewComponent;
  let fixture: ComponentFixture<LongItemViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LongItemViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LongItemViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
