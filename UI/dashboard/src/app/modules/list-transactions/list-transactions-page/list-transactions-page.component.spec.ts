import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListTransactionsPageComponent } from './list-transactions-page.component';

describe('ListTransactionsPageComponent', () => {
  let component: ListTransactionsPageComponent;
  let fixture: ComponentFixture<ListTransactionsPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ListTransactionsPageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ListTransactionsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
