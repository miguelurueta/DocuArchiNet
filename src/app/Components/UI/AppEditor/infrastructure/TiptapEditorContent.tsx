import { EditorContent } from "@tiptap/react";
import type { Editor } from "@tiptap/react";

type TiptapEditorContentProps = {
  editor: Editor | null;
  className?: string;
  "aria-labelledby"?: string;
  "aria-label"?: string;
  "aria-describedby"?: string;
  "aria-invalid"?: boolean;
};

export function TiptapEditorContent({
  editor,
  className,
  "aria-labelledby": ariaLabelledBy,
  "aria-label": ariaLabel,
  "aria-describedby": ariaDescribedBy,
  "aria-invalid": ariaInvalid,
}: TiptapEditorContentProps) {
  return (
    <EditorContent
      editor={editor}
      className={className}
      aria-labelledby={ariaLabelledBy}
      aria-label={ariaLabel}
      aria-describedby={ariaDescribedBy}
      aria-invalid={ariaInvalid}
    />
  );
}
