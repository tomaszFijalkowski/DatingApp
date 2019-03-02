import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AdminService } from '../_services/admin.service';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class PhotoManagementResolver implements Resolve<any> {

    constructor(private adminService: AdminService, private authService: AuthService,
        private router: Router, private toastr: ToastrService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<any> {
        if (this.authService.roleMatch(['Admin', 'Moderator'])) {
            return this.adminService.getPhotosForApproval().pipe(
                catchError(error => {
                    this.toastr.toastrConfig.preventDuplicates = true;
                    this.toastr.error('Could not retrieve Manage data');
                    this.router.navigate(['/home']);
                    return of(null);
                })
            );
        }
    }
}
