import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable, of, merge } from 'rxjs';
import { VirtualServer } from '../models/virtualServer';

const httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({ providedIn: 'root' })
export class ServersService {

    constructor(private http: HttpClient) { }

    getServers(): Observable<VirtualServer[]> {
        return this.http.get<VirtualServer[]>("api/servers/list");
    }

    addServer(): Observable<VirtualServer> {
        return this.http.post<VirtualServer>("api/servers/add", null);
    }

    selectForRemove(id: number, selected: boolean): Observable<any> {
        return this.http.post<any>(`api/servers/selectForRemove?id=${id}&selected=${selected}`, null);
    }

    removeSelected(): Observable<any> {
        return this.http.post<any>("api/servers/removeSelected", null);
    }
}

