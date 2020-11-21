import { ResearcherResearch } from './../_models/researcherResearch';
import { Researcher } from './../_models/researcher';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';
import { Research } from '../_models/research';

@Injectable({
  providedIn: 'root',
})
export class ResearchService {
  baseUrl = environment.apiUrl;
  researchers: Researcher[] = [];
  researches: Research[] = [];
  researcher: Researcher;

  constructor(private http: HttpClient) {}

  getResearchers() {
    return this.http
      .get<Researcher[]>(this.baseUrl + 'research/researchers')
      .pipe(
        map((researchers) => {
          this.researchers = researchers;
          console.log(this.researchers[0].firstname);
          return researchers;
        })
      );
  }

  getResearchersByResearchId(id: number) {
    return this.http
      .get<Researcher[]>(this.baseUrl + 'research/researchers/' + id)
      .pipe(
        map((researchers) => {
          this.researchers = researchers;
          return researchers;
        })
      );
  }

  getResearches() {
    return this.http.get<Research[]>(this.baseUrl + 'research/researches').pipe(
      map((researches) => {
        this.researches = researches;
        return researches;
      })
    );
  }

  getResearchesByResearcherId(id: number) {
    return this.http
      .get<Research[]>(this.baseUrl + 'research/researches/' + id)
      .pipe(
        map((researches) => {
          this.researches = researches;
          return researches;
        })
      );
  }

  setResearcher(res: Researcher) {
    this.researcher = res;
  }

  updateResearcher(res: Researcher) {
    return this.http
      .put(this.baseUrl + 'research/researchers/updateresearcher', res)
      .pipe(
        map(() => {
          const index = this.researchers.indexOf(res);
          this.researchers[index] = res;
        })
      );
  }

  deleteResearcher(re: Researcher) {
    return this.http.post<Researcher>(this.baseUrl + 'research/researchers/deleteresearcher', re).pipe(res => {
        return res;
    });
  }

  deleteResearcherFromResearch(re: ResearcherResearch) {
    return this.http.post(this.baseUrl + 'research/researchers/deletefromresearch', re).pipe();
  }
  addResearcher(re: Researcher) {
    return this.http.post<Researcher>(this.baseUrl + 'research/researchers/addresearcher', re).pipe( res => {
      return res;
    });
  }
}
