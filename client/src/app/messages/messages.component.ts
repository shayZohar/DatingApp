import { MessageService } from './../_services/message.service';
import { Pagination } from './../_models/pagination';
import { Message } from './../_models/message';
import { Component, OnInit } from '@angular/core';
import { ConfirmService } from '../_services/confirm.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss'],
})
export class MessagesComponent implements OnInit {
  messages: Message[] = [];
  pagination: Pagination;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;
  loadingFlag = false;

  constructor(private messageService: MessageService, private confirmService : ConfirmService) {}

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    this.loadingFlag = true;
    this.messageService
      .getMessages(this.pageNumber, this.pageSize, this.container)
      .subscribe((response) => {
        this.messages = response.result;
        this.pagination = response.pagination;
        this.loadingFlag = false;
      });
  }

  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadMessages();
  }

  deleteMessage(id: number) {
    this.confirmService.confirm('Confirm Delete Message', 'This cannot be undone').subscribe(result => {
      if(result) {
        this.messageService.deleteMessage(id).subscribe(() => {
          this.messages.splice(
            this.messages.findIndex((m) => m.id === id),
            1
          );
        });
      }
    });
  }
}
