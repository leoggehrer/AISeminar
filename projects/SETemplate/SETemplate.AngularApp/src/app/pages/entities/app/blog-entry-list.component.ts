//@Ignore
import { IdType, IdDefault } from '@app/models/i-key-model';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { IQueryParams } from '@app/models/base/i-query-params';
import { IBlogEntry } from '@app-models/entities/app/i-blog-entry';
import { BlogEntryBaseListComponent }from '@app/components/entities/app/blog-entry-base-list.component';
import { BlogEntryEditComponent }from '@app/components/entities/app/blog-entry-edit.component';
@Component({
  standalone: true,
  selector:'app-blog-entry-list',
  imports: [ CommonModule, FormsModule, TranslateModule, RouterModule ],
  templateUrl: './blog-entry-list.component.html',
  styleUrl: './blog-entry-list.component.css'
})
export class BlogEntryListComponent extends BlogEntryBaseListComponent {
  constructor()
  {
    super();
  }
  override ngOnInit(): void {
    super.ngOnInit();
    this.reloadData();
  }
  override prepareQueryParams(queryParams: IQueryParams): void {
    super.prepareQueryParams(queryParams);
    queryParams.filter = 'title.ToLower().Contains(@0) OR summary.ToLower().Contains(@0) OR content.ToLower().Contains(@0) OR author.ToLower().Contains(@0) OR tags.ToLower().Contains(@0)';
  }
  protected override getItemKey(item: IBlogEntry): IdType {
    return item?.id || IdDefault;
  }
  override get pageTitle(): string {
    return 'BlogEntries';
  }
  override getEditComponent() {
    return BlogEntryEditComponent;
  }
  public refreshCommand(): void {
    this.reloadData();
  }
}
