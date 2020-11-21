import { ResearcherResearch } from './../../_models/researcherResearch';
import { ToastrService } from 'ngx-toastr';
import { ResearchService } from './../../_services/research.service';
import { Researcher } from './../../_models/researcher';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Research } from 'src/app/_models/research';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss'],
})
export class EditComponent implements OnInit {
  researcher: Researcher;
  researchToDelete: Research;
  researchesAssignedTo: Research[];

  @ViewChild('editForm') editForm: NgForm;

  constructor(private resService: ResearchService, private toastr: ToastrService) {}

  ngOnInit(): void {
    if (!this.researcher){
      this.researcher = this.resService.researcher = JSON.parse(localStorage.getItem('currentResearcher'));
    }
    console.log('hey: ' + this.researcher.firstname );
    this.resService.getResearchesByResearcherId(this.researcher.id).subscribe(res => {
      this.researchesAssignedTo = res;
    });
  }

  update() {
    this.resService.updateResearcher(this.researcher).subscribe(() => {
      this.toastr.success('Profile updated successfuly!');
      // we reset the form state but keeps the updated values of the member
      this.editForm.reset(this.researcher);
    });
}

chosenResearchToDelete(research: Research) {
 this.researchToDelete = research;
}

deleteResearch() {
 const res: ResearcherResearch  =
 {researcherId: this.researcher.id, researchId: this.researchToDelete.id};
 this.resService.deleteResearcherFromResearch(res).subscribe();

 this.researchesAssignedTo = this.researchesAssignedTo.filter(val => val !== this.researchToDelete);

 this.researchToDelete = null;
}
}
