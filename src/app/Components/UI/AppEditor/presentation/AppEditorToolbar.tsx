import { memo, useEffect, useRef, useState } from "react";
import { Input, Popover } from "antd";
import { DownOutlined } from "@ant-design/icons";
import type { ChangeEvent } from "react";
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
  faChevronDown,
  faHeading,
  faGripLines,
} from "@fortawesome/free-solid-svg-icons";
import type { Editor } from "@tiptap/react";
import { AppDropdown } from "../../AppDropdown";
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
  { value: "paragraph", label: "Parrafo", shortLabel: "P", icon: faParagraph },
  { value: "h1", label: "Titulo 1", shortLabel: "H1", icon: faHeading },
  { value: "h2", label: "Titulo 2", shortLabel: "H2", icon: faHeading },
  { value: "h3", label: "Titulo 3", shortLabel: "H3", icon: faHeading },
] as const;

const TOOLBAR_GROUPS = {
  formatting: ["bold", "italic", "underline"],
  structure: ["bullet-list", "ordered-list", "task-list"],
  history: ["undo", "redo"],
  insert: ["link", "image", "page-break"],
} as const;

function useCompactToolbarMode(maxWidth = 1024) {
  const [isCompact, setIsCompact] = useState(
    () => typeof window !== "undefined" && window.innerWidth <= maxWidth,
  );

  useEffect(() => {
    if (typeof window === "undefined") {
      return undefined;
    }

    const handleResize = () => {
      setIsCompact(window.innerWidth <= maxWidth);
    };

    handleResize();
    window.addEventListener("resize", handleResize);

    return () => {
      window.removeEventListener("resize", handleResize);
    };
  }, [maxWidth]);

  return isCompact;
}

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

function getHeadingOption(value: string) {
  return HEADING_OPTIONS.find((option) => option.value === value) ?? HEADING_OPTIONS[0];
}

function getCurrentTextAlign(editor: Editor | null) {
  if (!editor) {
    return "left";
  }

  if (editor.isActive({ textAlign: "center" })) {
    return "center";
  }

  if (editor.isActive({ textAlign: "right" })) {
    return "right";
  }

  if (editor.isActive({ textAlign: "justify" })) {
    return "justify";
  }

  return "left";
}

function getTextAlignMeta(value: string) {
  switch (value) {
    case "center":
      return { key: "align-center", label: "Centro", icon: faAlignCenter };
    case "right":
      return { key: "align-right", label: "Derecha", icon: faAlignRight };
    case "justify":
      return { key: "align-justify", label: "Justificar", icon: faAlignJustify };
    default:
      return { key: "align-left", label: "Izquierda", icon: faAlignLeft };
  }
}

function canRun(editor: Editor | null, command: (instance: Editor) => boolean) {
  if (!editor) {
    return false;
  }

  return command(editor);
}

function canInsertPageBreak(editor: Editor | null) {
  if (!editor) {
    return false;
  }

  const canChain = editor.can().chain().focus() as {
    insertPageBreak?: () => { run: () => boolean };
  };

  return typeof canChain.insertPageBreak === "function" && canChain.insertPageBreak().run();
}

function runInsertPageBreak(editor: Editor | null) {
  if (!editor) {
    return;
  }

  const chain = editor.chain().focus() as {
    insertPageBreak?: () => { run: () => boolean };
  };

  if (typeof chain.insertPageBreak === "function") {
    chain.insertPageBreak().run();
  }
}

function formatUrl(value: string) {
  if (/^https?:\/\//i.test(value)) {
    return value;
  }

  return `https://${value}`;
}

function normalizeImageWidth(value: string) {
  const normalizedValue = value.trim();

  if (!normalizedValue) {
    return undefined;
  }

  if (/^\d+(\.\d+)?%$/.test(normalizedValue)) {
    return normalizedValue;
  }

  if (/^\d+(\.\d+)?px$/i.test(normalizedValue)) {
    return normalizedValue.toLowerCase();
  }

  if (/^\d+(\.\d+)?$/.test(normalizedValue)) {
    return `${normalizedValue}px`;
  }

  return normalizedValue;
}

function hasActiveImageSelection(editor: Editor | null) {
  if (!editor) {
    return false;
  }

  const selection = editor.state?.selection as
    | {
        node?: { type?: { name?: string } } | null;
        $anchor?: { parent?: { type?: { name?: string } } };
      }
    | undefined;

  if (editor.isActive("image")) {
    return true;
  }

  if (selection?.node?.type?.name === "image") {
    return true;
  }

  return selection?.$anchor?.parent?.type?.name === "image";
}

function AppEditorToolbarComponent({
  editor,
  disabled = false,
}: AppEditorToolbarProps) {
  const isBlocked = disabled || !editor;
  const isCompactToolbar = useCompactToolbarMode();
  const [isLinkPopoverOpen, setIsLinkPopoverOpen] = useState(false);
  const [isImagePopoverOpen, setIsImagePopoverOpen] = useState(false);
  const [linkValue, setLinkValue] = useState("");
  const [imageUrlValue, setImageUrlValue] = useState("");
  const [imageWidthValue, setImageWidthValue] = useState("");
  const fileInputRef = useRef<HTMLInputElement | null>(null);
  const isImageActive = hasActiveImageSelection(editor) || isImagePopoverOpen;

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

  const handleOpenLinkPopover = (open: boolean) => {
    if (disabled || !editor) {
      setIsLinkPopoverOpen(false);
      return;
    }

    if (open) {
      const currentHref = editor.getAttributes("link").href as string | undefined;
      setLinkValue(currentHref ?? "");
    }

    setIsLinkPopoverOpen(open);
  };

  const handleApplyLink = () => {
    if (!editor || disabled) {
      return;
    }

    const normalizedHref = linkValue.trim();
    if (!normalizedHref) {
      editor.chain().focus().extendMarkRange("link").unsetLink().run();
      setIsLinkPopoverOpen(false);
      return;
    }

    editor
      .chain()
      .focus()
      .extendMarkRange("link")
      .setLink({ href: formatUrl(normalizedHref) })
      .run();

    setIsLinkPopoverOpen(false);
  };

  const handleOpenImagePopover = (open: boolean) => {
    if (disabled || !editor) {
      setIsImagePopoverOpen(false);
      return;
    }

    if (open) {
      const currentWidth = editor.getAttributes("image").width as string | undefined;
      setImageWidthValue(currentWidth ?? "");
    }

    if (!open) {
      setImageUrlValue("");
      setImageWidthValue("");
    }

    setIsImagePopoverOpen(open);
  };

  const handleApplyImageUrl = () => {
    if (!editor || disabled) {
      return;
    }

    const normalizedWidth = normalizeImageWidth(imageWidthValue);
    const normalizedSrc = imageUrlValue.trim();
    if (!normalizedSrc && isImageActive) {
      editor
        .chain()
        .focus()
        .updateAttributes("image", {
          width: normalizedWidth ?? null,
        })
        .run();
      setIsImagePopoverOpen(false);
      return;
    }

    if (!normalizedSrc) {
      return;
    }

    editor
      .chain()
      .focus()
      .setImage({
        src: formatUrl(normalizedSrc),
      })
      .run();

    if (normalizedWidth) {
      editor
        .chain()
        .focus()
        .updateAttributes("image", {
          width: normalizedWidth,
        })
        .run();
    }

    setImageUrlValue("");
    setImageWidthValue("");
    setIsImagePopoverOpen(false);
  };

  const handleApplyImagePreset = (preset: string) => {
    if (!editor || disabled) {
      return;
    }

    setImageWidthValue(preset);

    if (isImageActive) {
      editor
        .chain()
        .focus()
        .updateAttributes("image", {
          width: preset,
        })
        .run();
    }
  };

  const handleImageFileChange = (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file || !editor || disabled) {
      return;
    }

    const normalizedWidth = normalizeImageWidth(imageWidthValue);

    const reader = new FileReader();
    reader.onload = () => {
      const result = typeof reader.result === "string" ? reader.result : "";
      if (!result) {
        return;
      }

      editor
        .chain()
        .focus()
        .setImage({
          src: result,
        })
        .run();

      if (normalizedWidth) {
        editor
          .chain()
          .focus()
          .updateAttributes("image", {
            width: normalizedWidth,
          })
          .run();
      }

      setImageUrlValue("");
      setImageWidthValue("");
      setIsImagePopoverOpen(false);
    };
    reader.readAsDataURL(file);
    event.target.value = "";
  };

  const linkPopoverContent = (
    <div className={styles.toolbarPopoverContent}>
      <label className={styles.toolbarPopoverLabel} htmlFor="app-editor-link-url">
        URL del enlace
      </label>
      <Input
        id="app-editor-link-url"
        value={linkValue}
        onChange={(event) => setLinkValue(event.target.value)}
        placeholder="https://ejemplo.com"
        disabled={isBlocked}
      />
      <div className={styles.toolbarPopoverActions}>
        <AppButton
          variant="ghost"
          size="sm"
          onClick={() => {
            setLinkValue("");
            if (!editor || disabled) {
              return;
            }

            editor.chain().focus().extendMarkRange("link").unsetLink().run();
            setIsLinkPopoverOpen(false);
          }}
          disabled={isBlocked}
        >
          Quitar
        </AppButton>
        <AppButton variant="primary" size="sm" onClick={handleApplyLink} disabled={isBlocked}>
          Aplicar
        </AppButton>
      </div>
    </div>
  );

  const imagePopoverContent = (
    <div className={styles.toolbarPopoverContent}>
      <label className={styles.toolbarPopoverLabel} htmlFor="app-editor-image-url">
        URL de la imagen
      </label>
      <Input
        id="app-editor-image-url"
        value={imageUrlValue}
        onChange={(event) => setImageUrlValue(event.target.value)}
        placeholder="https://cdn.ejemplo.com/imagen.png"
        disabled={isBlocked}
      />
      <label className={styles.toolbarPopoverLabel} htmlFor="app-editor-image-width">
        Ancho de la imagen
      </label>
      <Input
        id="app-editor-image-width"
        value={imageWidthValue}
        onChange={(event) => setImageWidthValue(event.target.value)}
        placeholder="100% o 480"
        disabled={isBlocked}
      />
      <div className={styles.toolbarPresetGroup} role="group" aria-label="Tamaños rapidos de imagen">
        {["25%", "50%", "75%", "100%"].map((preset) => (
          <AppButton
            key={preset}
            variant={imageWidthValue === preset ? "primary" : "ghost"}
            size="sm"
            onClick={() => handleApplyImagePreset(preset)}
            disabled={isBlocked}
          >
            {preset}
          </AppButton>
        ))}
      </div>
      <div className={styles.toolbarPopoverActions}>
        <AppButton
          variant="secondary"
          size="sm"
          onClick={() => fileInputRef.current?.click()}
          disabled={isBlocked}
        >
          Cargar archivo
        </AppButton>
        <AppButton variant="primary" size="sm" onClick={handleApplyImageUrl} disabled={isBlocked}>
          {isImageActive && !imageUrlValue.trim() ? "Aplicar tamaño" : "Insertar"}
        </AppButton>
      </div>
      <input
        ref={fileInputRef}
        type="file"
        accept="image/*"
        onChange={handleImageFileChange}
        className={styles.toolbarHiddenInput}
        tabIndex={-1}
      />
    </div>
  );

  const renderActionButton = (button: ToolbarButtonConfig) => {
    if (button.key === "link") {
      return (
        <Popover
          key={button.key}
          content={linkPopoverContent}
          trigger="click"
          placement="bottomLeft"
          open={isLinkPopoverOpen}
          onOpenChange={handleOpenLinkPopover}
        >
          <span>
            <AppButton
              variant={button.isActive ? "primary" : "ghost"}
              size="sm"
              icon={<FontAwesomeIcon icon={button.icon} />}
              aria-label={button.label}
              tooltip={button.label}
              disabled={button.disabled}
              className={joinClasses(button.isActive && styles.toolbarButtonActive)}
            />
          </span>
        </Popover>
      );
    }

    if (button.key === "image") {
      return (
        <Popover
          key={button.key}
          content={imagePopoverContent}
          trigger="click"
          placement="bottomLeft"
          open={isImagePopoverOpen}
          onOpenChange={handleOpenImagePopover}
        >
          <span>
            <AppButton
              variant={button.isActive ? "primary" : "ghost"}
              size="sm"
              icon={<FontAwesomeIcon icon={button.icon} />}
              aria-label={button.label}
              tooltip={button.label}
              disabled={button.disabled}
              className={joinClasses(button.isActive && styles.toolbarButtonActive)}
            />
          </span>
        </Popover>
      );
    }

    return (
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
    );
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
      onClick: () => undefined,
    },
    {
      key: "image",
      label: "Insertar imagen",
      icon: faImage,
      disabled: isBlocked,
      onClick: () => undefined,
    },
    {
      key: "page-break",
      label: "Insertar salto de pagina",
      icon: faGripLines,
      isActive: editor?.isActive("pageBreak"),
      disabled: isBlocked || !canInsertPageBreak(editor),
      onClick: () => runInsertPageBreak(editor),
    },
  ];

  const groupedButtons = {
    formatting: buttons.filter((button) => TOOLBAR_GROUPS.formatting.includes(button.key as never)),
    structure: buttons.filter((button) => TOOLBAR_GROUPS.structure.includes(button.key as never)),
    history: buttons.filter((button) => TOOLBAR_GROUPS.history.includes(button.key as never)),
    insert: buttons.filter((button) => TOOLBAR_GROUPS.insert.includes(button.key as never)),
  };

  const currentHeading = getHeadingOption(getCurrentHeadingValue(editor));
  const currentTextAlign = getTextAlignMeta(getCurrentTextAlign(editor));
  const headingItems = HEADING_OPTIONS.map((option) => ({
    key: option.value,
    label: option.label,
    leftIcon: <FontAwesomeIcon icon={option.icon} />,
    onSelect: () => handleHeadingChange(option.value),
  }));
  const alignItems = [
    {
      key: "align-left",
      label: "Izquierda",
      leftIcon: <FontAwesomeIcon icon={faAlignLeft} />,
      onSelect: () => editor?.chain().focus().setTextAlign("left").run(),
    },
    {
      key: "align-center",
      label: "Centro",
      leftIcon: <FontAwesomeIcon icon={faAlignCenter} />,
      onSelect: () => editor?.chain().focus().setTextAlign("center").run(),
    },
    {
      key: "align-right",
      label: "Derecha",
      leftIcon: <FontAwesomeIcon icon={faAlignRight} />,
      onSelect: () => editor?.chain().focus().setTextAlign("right").run(),
    },
    {
      key: "align-justify",
      label: "Justificar",
      leftIcon: <FontAwesomeIcon icon={faAlignJustify} />,
      onSelect: () => editor?.chain().focus().setTextAlign("justify").run(),
    },
  ];
  const structureItems = [
    {
      key: "bullet-list",
      label: "Lista con vietas",
      leftIcon: <FontAwesomeIcon icon={faListUl} />,
      onSelect: () => editor?.chain().focus().toggleBulletList().run(),
    },
    {
      key: "ordered-list",
      label: "Lista numerada",
      leftIcon: <FontAwesomeIcon icon={faListOl} />,
      onSelect: () => editor?.chain().focus().toggleOrderedList().run(),
    },
    {
      key: "task-list",
      label: "Lista de tareas",
      leftIcon: <FontAwesomeIcon icon={faListCheck} />,
      onSelect: () => editor?.chain().focus().toggleTaskList().run(),
    },
  ];

  const renderButtonGroup = (group: ToolbarButtonConfig[], label: string) => (
    <div className={styles.toolbarButtonGroup} role="group" aria-label={label}>
      {group.map((button) => renderActionButton(button))}
    </div>
  );

  return (
    <div
      className={styles.toolbar}
      role="toolbar"
      aria-label="Barra de herramientas del editor"
      data-toolbar-mode={isCompactToolbar ? "compact" : "default"}
    >
      <div className={styles.toolbarSection} data-group="heading">
        <AppDropdown
          trigger={
            <AppButton
              variant="ghost"
              size="sm"
              leftIcon={<FontAwesomeIcon icon={currentHeading.icon} />}
              rightIcon={<FontAwesomeIcon icon={faChevronDown} />}
              className={styles.headingButton}
              aria-label={`Nivel de encabezado actual: ${currentHeading.label}`}
            >
              {currentHeading.shortLabel}
            </AppButton>
          }
          items={headingItems}
          disabled={isBlocked}
          className={styles.headingDropdown}
          ariaLabel="Nivel de encabezado"
          placement="bottomLeft"
        />
      </div>

      <div className={styles.toolbarSection} data-group="actions">
        {renderButtonGroup(groupedButtons.formatting, "Formato de texto")}
        {isCompactToolbar ? (
          <div className={styles.toolbarButtonGroup} role="group" aria-label="Estructura de contenido">
            <AppDropdown
              trigger={
                <AppButton
                  variant="ghost"
                  size="sm"
                  leftIcon={<FontAwesomeIcon icon={faListUl} />}
                  rightIcon={<FontAwesomeIcon icon={faChevronDown} />}
                  className={styles.compactGroupButton}
                  aria-label="Estructura de contenido"
                >
                  <span className={styles.compactButtonLabel}>Bloques</span>
                </AppButton>
              }
              items={structureItems}
              disabled={isBlocked}
              ariaLabel="Estructura de contenido"
              placement="bottomLeft"
            />
          </div>
        ) : (
          renderButtonGroup(groupedButtons.structure, "Estructura de contenido")
        )}
        <div className={styles.toolbarButtonGroup} role="group" aria-label="Alineacion">
          <AppDropdown
            trigger={
              <AppButton
                variant="ghost"
                size="sm"
                leftIcon={<FontAwesomeIcon icon={currentTextAlign.icon} />}
                rightIcon={<DownOutlined />}
                className={styles.alignButton}
                aria-label={`Alineacion actual: ${currentTextAlign.label}`}
              >
                <span className={styles.toolbarCompactLabel}> </span>
              </AppButton>
            }
            items={alignItems}
            disabled={isBlocked}
            ariaLabel="Alineacion de texto"
            placement="bottomLeft"
          />
        </div>
        {renderButtonGroup(groupedButtons.history, "Historial de cambios")}
        {renderButtonGroup(groupedButtons.insert, "Insercion de contenido")}
      </div>
    </div>
  );
}

export const AppEditorToolbar = memo(AppEditorToolbarComponent);
