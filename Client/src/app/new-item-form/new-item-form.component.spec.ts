import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {NewItemFormComponentComponent} from './new-item-form-component.component';

describe('NewItemFormComponentComponent', () => {
  let component: NewItemFormComponentComponent;
  let fixture: ComponentFixture<NewItemFormComponentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewItemFormComponentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewItemFormComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
