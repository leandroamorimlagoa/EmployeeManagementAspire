/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { FrmMainComponent } from './frm-main.component';

describe('FrmMainComponent', () => {
  let component: FrmMainComponent;
  let fixture: ComponentFixture<FrmMainComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FrmMainComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FrmMainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
