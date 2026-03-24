//@Ignore
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { BlogEntryBaseEditComponent }from '@app/components/entities/app/blog-entry-base-edit.component';
@Component({
  standalone: true,
  selector:'app-blog-entry-edit',
  imports: [ CommonModule, FormsModule, TranslateModule],
  templateUrl: './blog-entry-edit.component.html',
  styleUrl: './blog-entry-edit.component.css'
})
export class BlogEntryEditComponent extends BlogEntryBaseEditComponent {
  override ngOnInit(): void {
    super.ngOnInit();
  }

  override get title(): string {
    return this.editMode ? 'BLOG_ENTRY_EDIT.EDIT_TITLE' : 'BLOG_ENTRY_EDIT.CREATE_TITLE';
  }
}
