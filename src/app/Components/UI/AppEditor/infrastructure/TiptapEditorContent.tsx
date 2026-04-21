import { EditorContent } from "@tiptap/react";
import type { Editor } from "@tiptap/react";
import type { CSSProperties } from "react";

type TiptapEditorContentProps = {
  editor: Editor | null;
  className?: string;
  style?: CSSProperties;
  "aria-labelledby"?: string;
  "aria-label"?: string;
  "aria-describedby"?: string;
  "aria-invalid"?: boolean;
};

export function TiptapEditorContent({
  editor,
  className,
  style,
  "aria-labelledby": ariaLabelledBy,
  "aria-label": ariaLabel,
  "aria-describedby": ariaDescribedBy,
  "aria-invalid": ariaInvalid,
}: TiptapEditorContentProps) {
  return (
    <EditorContent
      editor={editor}
      className={className}
      style={style}
      aria-labelledby={ariaLabelledBy}
      aria-label={ariaLabel}
      aria-describedby={ariaDescribedBy}
      aria-invalid={ariaInvalid}
    />
  );
}
