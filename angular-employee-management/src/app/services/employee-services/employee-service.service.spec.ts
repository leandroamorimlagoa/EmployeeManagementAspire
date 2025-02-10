/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { EmployeeServiceService } from './employee-service.service';

describe('Service: EmployeeService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [EmployeeServiceService]
    });
  });

  it('should ...', inject([EmployeeServiceService], (service: EmployeeServiceService) => {
    expect(service).toBeTruthy();
  }));
});
