import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {ComponentTestZoneComponent} from './component-test-zone.component';

describe('ComponentTestZoneComponent', () => {
  let component: ComponentTestZoneComponent;
  let fixture: ComponentFixture<ComponentTestZoneComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ComponentTestZoneComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ComponentTestZoneComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
