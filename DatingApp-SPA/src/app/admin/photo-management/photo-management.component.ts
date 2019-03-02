import { Component, OnInit, Input } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  @Input() photos: any;

  constructor(private adminService: AdminService, private toastr: ToastrService) { }

  ngOnInit() { }

  approvePhoto(photo) {
    this.adminService.approvePhoto(photo.id).subscribe(() => {
      this.photos.splice(this.photos.findIndex(p => p.id === photo.id), 1);
      this.toastr.success(photo.userName + '\'s photo has been approved');
    }, error => {
      this.toastr.error(error);
    });
  }

  rejectPhoto(photo) {
    this.adminService.rejectPhoto(photo.id).subscribe(() => {
      this.photos.splice(this.photos.findIndex(p => p.id === photo.id), 1);
      this.toastr.success(photo.userName + '\'s photo has been rejected');
    }, error => {
      this.toastr.error(error);
    });
  }

}
