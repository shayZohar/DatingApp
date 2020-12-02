import { Pagination } from 'src/app/_models/pagination';
import { MembersService } from './../_services/members.service';
import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.scss'],
})
export class ListsComponent implements OnInit {
  members: Partial<Member[]>;
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination;

  constructor(private memberService: MembersService) {}

  ngOnInit(): void {
    this.loadlikes();
  }

  loadlikes() {
    this.memberService.getLikes(this.predicate, this.pageNumber, this.pageSize).subscribe((response) => {
      this.members = response.result;
      this.pagination = response.pagination;
    });
  }

  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadlikes();
  }
}
