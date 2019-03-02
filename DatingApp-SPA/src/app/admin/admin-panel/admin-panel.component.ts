import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {
  photos: any;
  users: any;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.photos = this.route.snapshot.data['photos'];
    this.users = this.route.snapshot.data['users'];
  }

}
