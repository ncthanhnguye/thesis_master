import { Directive } from '@angular/core';

@Directive({
  selector: '[popupAnchor]',
  exportAs: 'popupAnchor'

})
export class PopupAnchorDirective {

  constructor() { }

}
