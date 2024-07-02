import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'truncate'
})
export class TruncatePipe implements PipeTransform {

  transform(value: string, args: any[]): unknown {
    //limit args[0]
    //add string args[1] at the end
    if(value){
      return value.length > args[0] ? value.substr(0, args[0]) + args[1] : value;
    }
  }

}
