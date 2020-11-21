import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { Researcher } from './../../_models/researcher';
import { ResearchService } from './../../_services/research.service';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { Research } from 'src/app/_models/research';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-research-main',
  templateUrl: './research-main.component.html',
  styleUrls: ['./research-main.component.scss'],
})
export class ResearchMainComponent implements OnInit {
  researchers: Researcher[] = [];
  researches: Research[] = [];
  modalRef: BsModalRef;
  researcher: Researcher = {
    firstname: 'First Name',
    lastname: 'Last Name',
    email: 'Email',
    phone: 'Phone',
  };
  show = false;

  constructor(
    private resService: ResearchService,
    private router: Router,
    private modalService: BsModalService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.resService.getResearchers().subscribe((researchers) => {
      this.researchers = researchers;
    });

    this.resService.getResearches().subscribe((researches) => {
      this.researches = researches;
    });
  }

  setCurrentResearcher(res: Researcher) {
    localStorage.setItem('currentResearcher', JSON.stringify(res));
    this.resService.setResearcher(res);
    console.log(res.lastname);
    this.router.navigateByUrl('research/edit-researcher');
  }

  addResearcher() {
    this.resService.addResearcher(this.researcher).subscribe((res) => {
      this.toastr.success('Researcher added!');
      this.researchers.push(this.researcher);
      this.modalService.hide();
    });
  }

  deleteResearcher(res: Researcher) {
    this.resService.deleteResearcher(res).subscribe(response => {
      this.researchers = this.researchers.filter(r => r.id !== res.id);
      this.toastr.success('Researcher deleted successfuly');
    });

  }

  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
  }
}
