import {
  HttpInterceptorFn,
  HttpRequest,
  HttpHandlerFn,
  HttpEvent,
  HttpErrorResponse
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ErrorModalComponent } from '../components/error-modal/error-modal.component';
import { AuthService } from '../services/auth-services/auth-service.service';

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> => {
  const router = inject(Router);
  const modalService = inject(NgbModal);
  const authService = inject(AuthService);

  const token = authService.getToken();

  let clonedRequest = req;
  if (token) {
    clonedRequest = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(clonedRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        authService.logout();
        router.navigate(['/login']);
      } else {
        // Exibe modal de erro
        showErrorModal(error, modalService);
      }
      return throwError(() => error);
    })
  );
};

function showErrorModal(error: HttpErrorResponse, modalService: NgbModal) {
  console.error('=====>',error);
  const modalRef = modalService.open(ErrorModalComponent, { centered: true });
  modalRef.componentInstance.message = error.error?.detail || 'Ocorreu um erro inesperado.';
}
