import { NgForm } from '@angular/forms';
import { MessageService } from './../../_services/message.service';
import { Message } from './../../_models/message';
import { ChangeDetectionStrategy, Component, Input, OnInit, ViewChild } from '@angular/core';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush, // this solve the error when message is added to bottom of scrolling window
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.scss'],
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm: NgForm;
  @Input() messages: Message[];
  @Input() username: string;
  messageContent: string;

  // we set it to be public so we can access it from the HTML template
  // we subscribe to the message thread in the template
  constructor(public messageService: MessageService) {}

  ngOnInit(): void {
  }

  sendMessage() {
    // because we use Promise in this method, we cannot subscribe to it, instead- we use 'then'
    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm.reset();
    });
  }
}
