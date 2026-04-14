import { memo } from "react";
import { Select } from "antd";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faBold,
  faItalic,
  faUnderline,
  faListUl,
  faListOl,
  faListCheck,
  faArrowRotateLeft,
  faArrowRotateRight,
  faLink,
  faImage,
  faAlignLeft,
  faAlignCenter,
  faAlignRight,
  faAlignJustify,
  faParagraph,
} from "@fortawesome/free-solid-svg-icons";
import type { Editor } from "@tiptap/react";
import { AppButton } from "../../AppButton";
import type { AppEditorHeadingLevel } from "../domain/editor.types";
import styles from "../AppEditor.module.css";

type AppEditorToolbarProps = {
  editor: Editor | null;
  disabled?: boolean;
};

type ToolbarButtonConfig = {
  key: string;
  label: string;
  icon: typeof faBold;
  isActive?: boolean;
  disabled?: boolean;
  onClick: () => void;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const HEADING_OPTIONS = [
  { value: "paragraph", label: "Parrafo", icon: faParagraph },
  { value: "h1", label: "Titulo 1", icon: faParagraph },
  { value: "h2", label: "Titulo 2", icon: faParagraph },
  { value: "h3", label: "Titulo 3", icon: faParagraph },
] as const;

function getCurrentHeadingValue(editor: Editor | null) {
  if (!editor) {
    return "paragraph";
  }

  if (editor.isActive("heading", { level: 1 })) {
    return "h1";
  }

  if (editor.isActive("heading", { level: 2 })) {
    return "h2";
  }

  if (editor.isActive("heading", { level: 3 })) {
    return "h3";
  }

  return "paragraph";
}

function canRun(editor: Editor | null, command: (instance: Editor) => boolean) {
  if (!editor) {
    return false;
  }

  return command(editor);
}

function formatUrl(value: string) {
  if (/^https?:\/\//i.test(value)) {
    return value;
  }

  return `https://${value}`;
}

function AppEditorToolbarComponent({ editor, disabled = false }: AppEditorToolbarProps) {
  const isBlocked = disabled || !editor;

  const handleHeadingChange = (value: string) => {
    if (!editor || disabled) {
      return;
    }

    if (value === "paragraph") {
      editor.chain().focus().setParagraph().run();
      return;
    }

    const level = Number(value.replace("h", "")) as AppEditorHeadingLevel;
    editor.chain().focus().toggleHeading({ level }).run();
  };

  const handleLinkClick = () => {
    if (!editor || disabled) {
      return;
    }

    const currentHref = editor.getAttributes("link").href as string | undefined;
    const nextHref = window.prompt("URL del enlace", currentHref ?? "https://");

    if (nextHref === null) {
      return;
    }

    const normalizedHref = nextHref.trim();
    if (!normalizedHref) {
      editor.chain().focus().extendMarkRange("link").unsetLink().run();
      return;
    }

    editor
      .chain()
      .focus()
      .extendMarkRange("link")
      .setLink({ href: formatUrl(normalizedHref) })
      .run();
  };

  const handleImageClick = () => {
    if (!editor || disabled) {
      return;
    }

    const nextSrc = window.prompt("URL de la imagen", "https://");
    if (!nextSrc) {
      return;
    }

    editor
      .chain()
      .focus()
      .setImage({ src: formatUrl(nextSrc.trim()) })
      .run();
  };

  const buttons: ToolbarButtonConfig[] = [
    {
      key: "bold",
      label: "Negrita",
      icon: faBold,
      isActive: editor?.isActive("bold"),
      disabled: isBlocked || !canRun(editor, (instance) => instance.can().chain().focus().toggleBold().run()),
      onClick: () => editor?.chain().focus().toggleBold().run(),
    },
    {
      key: "italic",
      label: "Cursiva",
      icon: faItalic,
      isActive: editor?.isActive("italic"),
      disabled: isBlocked || !canRun(editor, (instance) => instance.can().chain().focus().toggleItalic().run()),
      onClick: () => editor?.chain().focus().toggleItalic().run(),
    },
    {
      key: "underline",
      label: "Subrayado",
      icon: faUnderline,
      isActive: editor?.isActive("underline"),
      disabled: isBlocked || !canRun(editor, (instance) => instance.can().chain().focus().toggleUnderline().run()),
      onClick: () => editor?.chain().focus().toggleUnderline().run(),
    },
    {
      key: "bullet-list",
      label: "Lista con vietas",
      icon: faListUl,
      isActive: editor?.isActive("bulletList"),
      disabled:
        isBlocked || !canRun(editor, (instance) => instance.can().chain().focus().toggleBulletList().run()),
      onClick: () => editor?.chain().focus().toggleBulletList().run(),
    },
    {
      key: "ordered-list",
      label: "Lista numerada",
      icon: faListOl,
      isActive: editor?.isActive("orderedList"),
      disabled:
        isBlocked || !canRun(editor, (instance) => instance.can().chain().focus().toggleOrderedList().run()),
      onClick: () => editor?.chain().focus().toggleOrderedList().run(),
    },
    {
      key: "task-list",
      label: "Lista de tareas",
      icon: faListCheck,
      isActive: editor?.isActive("taskList"),
      disabled:
        isBlocked || !canRun(editor, (instance) => instance.can().chain().focus().toggleTaskList().run()),
      onClick: () => editor?.chain().focus().toggleTaskList().run(),
    },
    {
      key: "align-left",
      label: "Alinear a la izquierda",
      icon: faAlignLeft,
      isActive: editor?.isActive({ textAlign: "left" }),
      disabled: isBlocked,
      onClick: () => editor?.chain().focus().setTextAlign("left").run(),
    },
    {
      key: "align-center",
      label: "Centrar",
      icon: faAlignCenter,
      isActive: editor?.isActive({ textAlign: "center" }),
      disabled: isBlocked,
      onClick: () => editor?.chain().focus().setTextAlign("center").run(),
    },
    {
      key: "align-right",
      label: "Alinear a la derecha",
      icon: faAlignRight,
      isActive: editor?.isActive({ textAlign: "right" }),
      disabled: isBlocked,
      onClick: () => editor?.chain().focus().setTextAlign("right").run(),
    },
    {
      key: "align-justify",
      label: "Justificar",
      icon: faAlignJustify,
      isActive: editor?.isActive({ textAlign: "justify" }),
      disabled: isBlocked,
      onClick: () => editor?.chain().focus().setTextAlign("justify").run(),
    },
    {
      key: "undo",
      label: "Deshacer",
      icon: faArrowRotateLeft,
      disabled: isBlocked || !canRun(editor, (instance) => instance.can().chain().focus().undo().run()),
      onClick: () => editor?.chain().focus().undo().run(),
    },
    {
      key: "redo",
      label: "Rehacer",
      icon: faArrowRotateRight,
      disabled: isBlocked || !canRun(editor, (instance) => instance.can().chain().focus().redo().run()),
      onClick: () => editor?.chain().focus().redo().run(),
    },
    {
      key: "link",
      label: "Insertar enlace",
      icon: faLink,
      isActive: editor?.isActive("link"),
      disabled: isBlocked,
      onClick: handleLinkClick,
    },
    {
      key: "image",
      label: "Insertar imagen",
      icon: faImage,
      disabled: isBlocked,
      onClick: handleImageClick,
    },
  ];

  return (
    <div className={styles.toolbar} role="toolbar" aria-label="Barra de herramientas del editor">
      <div className={styles.toolbarSection}>
        <Select
          value={getCurrentHeadingValue(editor)}
          onChange={handleHeadingChange}
          disabled={isBlocked}
          className={styles.headingSelect}
          options={HEADING_OPTIONS.map((option) => ({
            value: option.value,
            label: option.label,
          }))}
          aria-label="Nivel de encabezado"
        />
      </div>

      <div className={styles.toolbarSection}>
        {buttons.map((button) => (
          <AppButton
            key={button.key}
            variant={button.isActive ? "primary" : "ghost"}
            size="sm"
            icon={<FontAwesomeIcon icon={button.icon} />}
            aria-label={button.label}
            tooltip={button.label}
            disabled={button.disabled}
            onClick={button.onClick}
            className={joinClasses(button.isActive && styles.toolbarButtonActive)}
          />
        ))}
      </div>
    </div>
  );
}

export const AppEditorToolbar = memo(AppEditorToolbarComponent);
