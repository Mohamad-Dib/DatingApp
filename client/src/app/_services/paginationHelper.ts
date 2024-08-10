import { HttpResponse ,HttpParams} from "@angular/common/http";
import { PaginatedResult } from "../_models/pagination";
import { signal } from "@angular/core";

export function setPaginatedRespone<T>(response:HttpResponse<T>,PaginatedResultSignal:ReturnType<typeof signal<PaginatedResult<T>|null>>)
{
  PaginatedResultSignal.set({
    items: response.body as T,
    pagination: JSON.parse(response.headers.get('Pagination')!)
  })
}

export function setPaginationHeaders(pageNumber:number,pageSize :number)
{
  let params = new HttpParams();
  if(pageNumber &&pageSize){
    params = params.append('pageNumber',pageNumber);
    params = params.append('pageSize',pageSize);
  }
  return params;
}