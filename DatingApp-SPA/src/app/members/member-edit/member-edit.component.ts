import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;
  user: User;
  photoUrl: string;
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(private route: ActivatedRoute, private toastr: ToastrService,
    private userService: UserService, private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
    this.toastr.toastrConfig.preventDuplicates = true;
  }

  updateUser() {
    this.toastr.clear();
    this.toastr.toastrConfig.easeTime = 300;
    this.toastr.toastrConfig.disableTimeOut = false;
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(next => {
      this.toastr.success('Profile updated successfully');
    }, error => {
      this.toastr.info(error);
    });
    this.editForm.reset(this.user);
  }

  updateMainPhoto(photoUrl) {
    this.user.photoUrl = photoUrl;
  }

  changesNotification() {
    this.toastr.toastrConfig.easeTime = 0;
    this.toastr.toastrConfig.disableTimeOut = true;
    this.toastr.info('Any unsaved changes will be lost');
  }
}
