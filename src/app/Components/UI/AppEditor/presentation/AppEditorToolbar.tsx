import { memo, useCallback, useEffect, useMemo, useRef, useState } from "react";
import { Input, Popover } from "antd";
import { DownOutlined } from "@ant-design/icons";
import type { ChangeEvent, MouseEvent, ReactNode } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Fragment } from "@tiptap/pm/model";
import { NodeSelection } from "@tiptap/pm/state";
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
  faArrowUp,
  faArrowDown,
} from "@fortawesome/free-solid-svg-icons";
import type { Editor } from "@tiptap/react";
import { AppDropdown } from "../../AppDropdown";
import { AppButton } from "../../AppButton";
import type { AppEditorHeadingLevel } from "../domain/editor.types";
import { clampSelection } from "../domain/editor.model";
import { generateEditorImageId } from "../application/localImageIds";
import styles from "../AppEditor.module.css";

type AppEditorToolbarProps = {
  editor: Editor | null;
  disabled?: boolean;
  onInsertLocalImage?: (file: File, width?: string) => Promise<void>;
  toolbarActions?: ReactNode;
  trailingContent?: ReactNode;
};

type ToolbarButtonConfig = {
  key: string;
  label: string;
  icon: typeof faBold;
  isActive?: boolean;
  disabled?: boolean;
  onClick: () => void;
};

type EditorWithImagePosition = Editor & {
  __appEditorLastImagePos?: number | null;
  __appEditorLastImageIdentity?: {
    imageId?: string | null;
    localImageId?: string | null;
    src?: string | null;
  } | null;
};

type TextSelectionRange = {
  from: number;
  to: number;
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
  insert: ["link", "image"],
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

function canSetImageAlign(editor: Editor | null) {
  if (editor?.isActive("image")) {
    return true;
  }

  return getResolvedImagePosition(editor) !== null;
}

function canMoveResolvedImage(editor: Editor | null, direction: "up" | "down") {
  if (!editor?.state?.doc) {
    return false;
  }

  const position = getResolvedImagePosition(editor);
  if (position === null) {
    return false;
  }

  const entries: Array<{ pos: number; nodeSize: number; typeName: string }> = [];
  editor.state.doc.forEach((node, offset) => {
    entries.push({
      pos: offset,
      nodeSize: node.nodeSize,
      typeName: node.type.name,
    });
  });

  const imageIndex = entries.findIndex(
    (entry) => entry.pos === position && entry.typeName === "image",
  );

  if (imageIndex === -1) {
    return false;
  }

  return direction === "up" ? imageIndex > 0 : imageIndex < entries.length - 1;
}

function moveResolvedImage(editor: Editor | null, direction: "up" | "down") {
  if (!editor?.state?.doc) {
    return false;
  }

  const position = getResolvedImagePosition(editor);
  if (position === null) {
    return false;
  }

  const entries: Array<{ pos: number; node: NonNullable<ReturnType<Editor["state"]["doc"]["nodeAt"]>> }> = [];
  editor.state.doc.forEach((node, offset) => {
    entries.push({
      pos: offset,
      node,
    });
  });

  const imageIndex = entries.findIndex(
    (entry) => entry.pos === position && entry.node.type.name === "image",
  );

  if (imageIndex === -1) {
    return false;
  }

  const targetIndex = direction === "up" ? imageIndex - 1 : imageIndex + 1;
  if (targetIndex < 0 || targetIndex >= entries.length) {
    return false;
  }

  const nextNodes = entries.map((entry) => entry.node);
  const [imageNode] = nextNodes.splice(imageIndex, 1);
  nextNodes.splice(targetIndex, 0, imageNode);

  let transaction = editor.state.tr.replaceWith(
    0,
    editor.state.doc.content.size,
    Fragment.fromArray(nextNodes),
  );

  let nextImagePosition = 0;
  for (let index = 0; index < targetIndex; index += 1) {
    nextImagePosition += nextNodes[index].nodeSize;
  }

  transaction = transaction.setSelection(
    NodeSelection.create(transaction.doc, nextImagePosition),
  );
  editor.view.dispatch(transaction);

  (editor as EditorWithImagePosition).__appEditorLastImagePos = nextImagePosition;
  (editor as EditorWithImagePosition).__appEditorLastImageIdentity = {
    imageId: typeof imageNode.attrs.imageId === "string" ? imageNode.attrs.imageId : null,
    localImageId:
      typeof imageNode.attrs.localImageId === "string" ? imageNode.attrs.localImageId : null,
    src: typeof imageNode.attrs.src === "string" ? imageNode.attrs.src : null,
  };

  return true;
}

function runHistoryCommand(editor: Editor | null, action: "undo" | "redo") {
  if (!editor) {
    return;
  }

  const commands = (editor.commands ?? {}) as {
    undo?: () => boolean;
    redo?: () => boolean;
  };

  const directCommand = commands[action];
  if (typeof directCommand === "function") {
    directCommand.call(commands);
    return;
  }

  const chain = editor.chain() as {
    undo?: () => { run: () => boolean };
    redo?: () => { run: () => boolean };
  };

  const chainedCommand = chain[action];
  if (typeof chainedCommand === "function") {
    chainedCommand.call(chain).run();
  }
}

function runSetImageAlign(editor: Editor | null, align: "left" | "center" | "right") {
  if (!editor) {
    return;
  }

  if (getResolvedImagePosition(editor) !== null) {
    updateResolvedImageAttributes(editor, { align });
    return;
  }

  const chain = editor.chain() as {
    setImageAlign?: (value: "left" | "center" | "right") => { run: () => boolean };
  };

  if (editor.isActive("image") && typeof chain.setImageAlign === "function") {
    chain.setImageAlign(align).run();
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
  if (editor?.isActive("image")) {
    return true;
  }

  return getResolvedImagePosition(editor) !== null;
}

function getLastImagePosition(editor: Editor | null) {
  if (!editor?.state?.doc) {
    return null;
  }

  let lastImagePosition: number | null = null;

  editor.state.doc.descendants((node, pos) => {
    if (node.type.name === "image") {
      lastImagePosition = pos;
    }
  });

  return lastImagePosition;
}

function getResolvedImagePosition(editor: Editor | null) {
  if (!editor || !editor.state?.doc) {
    return null;
  }

  const selection = editor.state?.selection as
    | {
        from?: number;
        node?: { type?: { name?: string } } | null;
        $anchor?: { parent?: { type?: { name?: string } } };
      }
    | undefined;

  if (selection?.node?.type?.name === "image" && typeof selection.from === "number") {
    return selection.from;
  }

  const lastImagePosition = (editor as EditorWithImagePosition).__appEditorLastImagePos;
  if (
    typeof lastImagePosition === "number" &&
    editor.state.doc.nodeAt(lastImagePosition)?.type.name === "image"
  ) {
    return lastImagePosition;
  }

  const lastImageIdentity = (editor as EditorWithImagePosition).__appEditorLastImageIdentity;
  if (lastImageIdentity) {
    let matchedPosition: number | null = null;

    editor.state.doc.descendants((node, pos) => {
      if (matchedPosition !== null || node.type.name !== "image") {
        return false;
      }

      const nodeImageId = typeof node.attrs.imageId === "string" ? node.attrs.imageId : null;
      const nodeLocalImageId =
        typeof node.attrs.localImageId === "string" ? node.attrs.localImageId : null;
      const nodeSrc = typeof node.attrs.src === "string" ? node.attrs.src : null;

      if (
        (lastImageIdentity.imageId && nodeImageId === lastImageIdentity.imageId) ||
        (lastImageIdentity.localImageId &&
          nodeLocalImageId === lastImageIdentity.localImageId) ||
        (lastImageIdentity.src && nodeSrc === lastImageIdentity.src)
      ) {
        matchedPosition = pos;
        return false;
      }

      return undefined;
    });

    if (matchedPosition !== null) {
      return matchedPosition;
    }
  }

  let imageCount = 0;
  let singleImagePosition: number | null = null;

  editor.state.doc.descendants((node, pos) => {
    if (node.type.name !== "image") {
      return;
    }

    imageCount += 1;
    if (imageCount === 1) {
      singleImagePosition = pos;
    }
  });

  if (imageCount === 1) {
    return singleImagePosition;
  }

  return getLastImagePosition(editor);
}

function getResolvedImageAttributes(editor: Editor | null) {
  const position = getResolvedImagePosition(editor);

  if (!editor || position === null) {
    return null;
  }

  const node = editor.state.doc.nodeAt(position);
  if (!node || node.type.name !== "image") {
    return null;
  }

  return node.attrs as Record<string, unknown>;
}

function updateResolvedImageAttributes(
  editor: Editor | null,
  attrs: Record<string, unknown>,
) {
  const position = getResolvedImagePosition(editor);

  if (!editor || position === null) {
    return false;
  }

  const node = editor.state.doc.nodeAt(position);
  if (!node || node.type.name !== "image") {
    return false;
  }

  let transaction = editor.state.tr.setNodeMarkup(position, undefined, {
    ...node.attrs,
    ...attrs,
  });

  const nextNode = transaction.doc.nodeAt(position);
  if (nextNode?.type.name === "image") {
    transaction = transaction.setSelection(NodeSelection.create(transaction.doc, position));
  }

  editor.view.dispatch(transaction);
  (editor as EditorWithImagePosition).__appEditorLastImagePos = position;
  (editor as EditorWithImagePosition).__appEditorLastImageIdentity = {
    imageId: typeof node.attrs.imageId === "string" ? node.attrs.imageId : null,
    localImageId: typeof node.attrs.localImageId === "string" ? node.attrs.localImageId : null,
    src: typeof node.attrs.src === "string" ? node.attrs.src : null,
  };
  return true;
}

function AppEditorToolbarComponent({
  editor,
  disabled = false,
  onInsertLocalImage,
  toolbarActions,
  trailingContent,
}: AppEditorToolbarProps) {
  const isBlocked = disabled || !editor;
  const [, setEditorSnapshotVersion] = useState(0);
  const isCompactToolbar = useCompactToolbarMode();
  const [isLinkPopoverOpen, setIsLinkPopoverOpen] = useState(false);
  const [isImagePopoverOpen, setIsImagePopoverOpen] = useState(false);
  const [isAlignDropdownOpen, setIsAlignDropdownOpen] = useState(false);
  const [linkValue, setLinkValue] = useState("");
  const [imageUrlValue, setImageUrlValue] = useState("");
  const [imageWidthValue, setImageWidthValue] = useState("");
  const fileInputRef = useRef<HTMLInputElement | null>(null);
  const alignSelectionRef = useRef<{ from: number; to: number } | null>(null);
  const textSelectionRef = useRef<TextSelectionRange | null>(null);
  const hasSelectedImage = hasActiveImageSelection(editor);

  useEffect(() => {
    if (!editor || typeof (editor as { on?: unknown }).on !== "function") {
      return undefined;
    }

    const syncToolbarState = () => {
      const selection = editor.state?.selection;
      if (
        selection &&
        typeof selection.from === "number" &&
        typeof selection.to === "number" &&
        selection.from !== selection.to &&
        selection.node?.type?.name !== "image"
      ) {
        textSelectionRef.current = {
          from: selection.from,
          to: selection.to,
        };
      }

      setEditorSnapshotVersion((currentVersion) => currentVersion + 1);
    };

    editor.on("transaction", syncToolbarState);
    editor.on("selectionUpdate", syncToolbarState);
    editor.on("focus", syncToolbarState);
    editor.on("blur", syncToolbarState);

    return () => {
      if (typeof (editor as { off?: unknown }).off !== "function") {
        return;
      }

      editor.off("transaction", syncToolbarState);
      editor.off("selectionUpdate", syncToolbarState);
      editor.off("focus", syncToolbarState);
      editor.off("blur", syncToolbarState);
    };
  }, [editor]);

  const handleToolbarMouseDownCapture = useCallback((event: MouseEvent<HTMLDivElement>) => {
    const target = event.target;

    if (!(target instanceof HTMLElement)) {
      return;
    }

    if (
      target.closest("input") ||
      target.closest("textarea") ||
      target.closest('[contenteditable="true"]') ||
      target.closest('[role="menu"]') ||
      target.closest(".ant-popover") ||
      target.closest(".ant-dropdown")
    ) {
      return;
    }

    const selection = editor?.state?.selection;
    if (
      selection &&
      typeof selection.from === "number" &&
      typeof selection.to === "number" &&
      selection.from !== selection.to &&
      selection.node?.type?.name !== "image"
    ) {
      textSelectionRef.current = {
        from: selection.from,
        to: selection.to,
      };
    }

    event.preventDefault();
  }, [editor]);

  const runWithPreservedTextSelection = useCallback(
    (applyCommand: (chain: Editor["chain"] extends () => infer T ? T : never) => { run: () => boolean }) => {
      if (!editor || disabled) {
        return;
      }

      const maxPosition = editor.state?.doc?.content?.size;
      const currentSelection =
        editor.state?.selection &&
        typeof editor.state.selection.from === "number" &&
        typeof editor.state.selection.to === "number" &&
        editor.state.selection.from !== editor.state.selection.to &&
        editor.state.selection.node?.type?.name !== "image"
          ? {
              from: editor.state.selection.from,
              to: editor.state.selection.to,
            }
          : null;
      const savedSelection = currentSelection ?? textSelectionRef.current;
      let chain = editor.chain().focus() as Editor["chain"] extends () => infer T ? T : never;

      if (
        savedSelection &&
        typeof maxPosition === "number" &&
        typeof (chain as { setTextSelection?: unknown }).setTextSelection === "function"
      ) {
        chain = (chain as {
          setTextSelection: (selection: TextSelectionRange) => Editor["chain"] extends () => infer T ? T : never;
        }).setTextSelection({
          from: clampSelection(savedSelection.from, maxPosition),
          to: clampSelection(savedSelection.to, maxPosition),
        });
      }

      applyCommand(chain).run();
    },
    [disabled, editor],
  );

  const handleHeadingChange = useCallback((value: string) => {
    if (!editor || disabled) {
      return;
    }

    if (value === "paragraph") {
      editor.chain().focus().setParagraph().run();
      return;
    }

    const level = Number(value.replace("h", "")) as AppEditorHeadingLevel;
    editor.chain().focus().toggleHeading({ level }).run();
  }, [disabled, editor]);

  const handleOpenLinkPopover = useCallback((open: boolean) => {
    if (disabled || !editor) {
      setIsLinkPopoverOpen(false);
      return;
    }

    if (open) {
      const currentHref = editor.getAttributes("link").href as string | undefined;
      setLinkValue(currentHref ?? "");
    }

    setIsLinkPopoverOpen(open);
  }, [disabled, editor]);

  const handleApplyLink = useCallback(() => {
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
  }, [disabled, editor, linkValue]);

  const handleOpenImagePopover = useCallback((open: boolean) => {
    if (disabled || !editor) {
      setIsImagePopoverOpen(false);
      return;
    }

    if (open) {
      const resolvedPosition = getResolvedImagePosition(editor);
      if (resolvedPosition !== null) {
        const resolvedNode = editor.state.doc.nodeAt(resolvedPosition);
        if (resolvedNode?.type.name === "image") {
          (editor as EditorWithImagePosition).__appEditorLastImagePos = resolvedPosition;
          (editor as EditorWithImagePosition).__appEditorLastImageIdentity = {
            imageId:
              typeof resolvedNode.attrs.imageId === "string"
                ? resolvedNode.attrs.imageId
                : null,
            localImageId:
              typeof resolvedNode.attrs.localImageId === "string"
                ? resolvedNode.attrs.localImageId
                : null,
            src: typeof resolvedNode.attrs.src === "string" ? resolvedNode.attrs.src : null,
          };
        }
      }

      const currentImageAttributes =
        getResolvedImageAttributes(editor) ??
        (editor.getAttributes("image") as Record<string, unknown>);
      const currentWidth = currentImageAttributes?.width as string | undefined;
      setImageWidthValue(currentWidth ?? "");
    }

    if (!open) {
      setImageUrlValue("");
      setImageWidthValue("");
    }

    setIsImagePopoverOpen(open);
  }, [disabled, editor]);

  const handleAlignDropdownOpenChange = useCallback((open: boolean) => {
    if (
      open &&
      editor &&
      editor.state?.selection &&
      typeof editor.state.selection.from === "number" &&
      typeof editor.state.selection.to === "number"
    ) {
      alignSelectionRef.current = {
        from: editor.state.selection.from,
        to: editor.state.selection.to,
      };
    } else if (open) {
      alignSelectionRef.current = null;
    }

    setIsAlignDropdownOpen(open);
  }, [editor]);

  const applySavedTextAlign = useCallback((align: "left" | "center" | "right" | "justify") => {
    if (!editor || disabled) {
      return;
    }

    const savedSelection = alignSelectionRef.current;
    const maxPosition = editor.state?.doc?.content?.size;
    const chain = editor.chain().focus() as {
      setTextSelection?: (selection: { from: number; to: number }) => {
        setTextAlign: (value: "left" | "center" | "right" | "justify") => { run: () => boolean };
      };
      setTextAlign: (value: "left" | "center" | "right" | "justify") => { run: () => boolean };
    };

    if (
      savedSelection &&
      typeof maxPosition === "number" &&
      typeof chain.setTextSelection === "function"
    ) {
      chain
        .setTextSelection({
          from: clampSelection(savedSelection.from, maxPosition),
          to: clampSelection(savedSelection.to, maxPosition),
        })
        .setTextAlign(align)
        .run();
    } else {
      chain.setTextAlign(align).run();
    }

    setIsAlignDropdownOpen(false);
  }, [disabled, editor]);

  const handleApplyImageUrl = useCallback(() => {
    if (!editor || disabled) {
      return;
    }

    const normalizedWidth = normalizeImageWidth(imageWidthValue);
    const normalizedSrc = imageUrlValue.trim();
    if (!normalizedSrc && hasSelectedImage) {
      if (editor.isActive("image")) {
        editor
          .chain()
          .focus()
          .updateAttributes("image", {
            width: normalizedWidth ?? null,
          })
          .run();
      } else {
        updateResolvedImageAttributes(editor, {
          width: normalizedWidth ?? null,
        });
      }
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
        imageId: generateEditorImageId(),
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
  }, [disabled, editor, hasSelectedImage, imageUrlValue, imageWidthValue]);

  const handleApplyImagePreset = useCallback((preset: string) => {
    if (!editor || disabled) {
      return;
    }

    setImageWidthValue(preset);

    if (hasSelectedImage) {
      if (editor.isActive("image")) {
        editor
          .chain()
          .focus()
          .updateAttributes("image", {
            width: preset,
          })
          .run();
      } else {
        updateResolvedImageAttributes(editor, {
          width: preset,
        });
      }
    }
  }, [disabled, editor, hasSelectedImage]);

  const handleImageFileChange = useCallback((event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file || !editor || disabled) {
      return;
    }

    const normalizedWidth = normalizeImageWidth(imageWidthValue);

    if (onInsertLocalImage) {
      void onInsertLocalImage(file, normalizedWidth);
    }

    event.target.value = "";
    setImageUrlValue("");
    setImageWidthValue("");
    setIsImagePopoverOpen(false);
  }, [disabled, editor, imageWidthValue, onInsertLocalImage]);

  const linkPopoverContent = useMemo(() => (
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
  ), [editor, disabled, handleApplyLink, isBlocked, linkValue]);

  const imagePopoverContent = useMemo(() => (
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
      {hasSelectedImage ? (
        <>
          <label className={styles.toolbarPopoverLabel}>Alineacion horizontal</label>
          <div
            className={styles.toolbarPresetGroup}
            role="group"
            aria-label="Alineacion horizontal de imagen"
          >
            <AppButton
              variant="ghost"
              size="sm"
              aria-label="Izquierda"
              onClick={() => runSetImageAlign(editor, "left")}
              disabled={isBlocked || !canSetImageAlign(editor)}
            >
              <FontAwesomeIcon icon={faAlignLeft} />
            </AppButton>
            <AppButton
              variant="ghost"
              size="sm"
              aria-label="Centro"
              onClick={() => runSetImageAlign(editor, "center")}
              disabled={isBlocked || !canSetImageAlign(editor)}
            >
              <FontAwesomeIcon icon={faAlignCenter} />
            </AppButton>
            <AppButton
              variant="ghost"
              size="sm"
              aria-label="Derecha"
              onClick={() => runSetImageAlign(editor, "right")}
              disabled={isBlocked || !canSetImageAlign(editor)}
            >
              <FontAwesomeIcon icon={faAlignRight} />
            </AppButton>
          </div>
          <label className={styles.toolbarPopoverLabel}>Mover imagen</label>
          <div
            className={styles.toolbarPresetGroup}
            role="group"
            aria-label="Movimiento vertical de imagen"
          >
            <AppButton
              variant="ghost"
              size="sm"
              aria-label="Mover arriba"
              onClick={() => moveResolvedImage(editor, "up")}
              disabled={isBlocked || !canMoveResolvedImage(editor, "up")}
            >
              <FontAwesomeIcon icon={faArrowUp} />
            </AppButton>
            <AppButton
              variant="ghost"
              size="sm"
              aria-label="Mover abajo"
              onClick={() => moveResolvedImage(editor, "down")}
              disabled={isBlocked || !canMoveResolvedImage(editor, "down")}
            >
              <FontAwesomeIcon icon={faArrowDown} />
            </AppButton>
          </div>
        </>
      ) : null}
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
          {hasSelectedImage && !imageUrlValue.trim() ? "Aplicar tamaño" : "Insertar"}
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
  ), [
    disabled,
    editor,
    handleApplyImagePreset,
    handleApplyImageUrl,
    handleImageFileChange,
    hasSelectedImage,
    imageUrlValue,
    imageWidthValue,
    isBlocked,
  ]);

  const renderActionButton = useCallback((button: ToolbarButtonConfig) => {
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
  }, [handleOpenImagePopover, handleOpenLinkPopover, imagePopoverContent, isImagePopoverOpen, isLinkPopoverOpen, linkPopoverContent]);

  const buttons: ToolbarButtonConfig[] = useMemo(() => [
    {
      key: "bold",
      label: "Negrita",
      icon: faBold,
      isActive: editor?.isActive("bold"),
      disabled: isBlocked || !canRun(editor, (instance) => instance.can().chain().focus().toggleBold().run()),
      onClick: () => runWithPreservedTextSelection((chain) => chain.toggleBold()),
    },
    {
      key: "italic",
      label: "Cursiva",
      icon: faItalic,
      isActive: editor?.isActive("italic"),
      disabled: isBlocked || !canRun(editor, (instance) => instance.can().chain().focus().toggleItalic().run()),
      onClick: () => runWithPreservedTextSelection((chain) => chain.toggleItalic()),
    },
    {
      key: "underline",
      label: "Subrayado",
      icon: faUnderline,
      isActive: editor?.isActive("underline"),
      disabled: isBlocked || !canRun(editor, (instance) => instance.can().chain().focus().toggleUnderline().run()),
      onClick: () => runWithPreservedTextSelection((chain) => chain.toggleUnderline()),
    },
    {
      key: "bullet-list",
      label: "Lista con vietas",
      icon: faListUl,
      isActive: editor?.isActive("bulletList"),
      disabled:
        isBlocked || !canRun(editor, (instance) => instance.can().chain().focus().toggleBulletList().run()),
      onClick: () => runWithPreservedTextSelection((chain) => chain.toggleBulletList()),
    },
    {
      key: "ordered-list",
      label: "Lista numerada",
      icon: faListOl,
      isActive: editor?.isActive("orderedList"),
      disabled:
        isBlocked || !canRun(editor, (instance) => instance.can().chain().focus().toggleOrderedList().run()),
      onClick: () => runWithPreservedTextSelection((chain) => chain.toggleOrderedList()),
    },
    {
      key: "task-list",
      label: "Lista de tareas",
      icon: faListCheck,
      isActive: editor?.isActive("taskList"),
      disabled:
        isBlocked || !canRun(editor, (instance) => instance.can().chain().focus().toggleTaskList().run()),
      onClick: () => runWithPreservedTextSelection((chain) => chain.toggleTaskList()),
    },
    {
      key: "undo",
      label: "Deshacer",
      icon: faArrowRotateLeft,
      disabled: isBlocked,
      onClick: () => runHistoryCommand(editor, "undo"),
    },
    {
      key: "redo",
      label: "Rehacer",
      icon: faArrowRotateRight,
      disabled: isBlocked,
      onClick: () => runHistoryCommand(editor, "redo"),
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
  ], [editor, isBlocked, runWithPreservedTextSelection]);

  const groupedButtons = useMemo(() => ({
    formatting: buttons.filter((button) => TOOLBAR_GROUPS.formatting.includes(button.key as never)),
    structure: buttons.filter((button) => TOOLBAR_GROUPS.structure.includes(button.key as never)),
    history: buttons.filter((button) => TOOLBAR_GROUPS.history.includes(button.key as never)),
    insert: buttons.filter((button) => TOOLBAR_GROUPS.insert.includes(button.key as never)),
  }), [buttons]);

  const currentHeading = useMemo(
    () => getHeadingOption(getCurrentHeadingValue(editor)),
    [editor],
  );
  const currentTextAlign = useMemo(
    () => getTextAlignMeta(getCurrentTextAlign(editor)),
    [editor],
  );
  const headingItems = useMemo(() => HEADING_OPTIONS.map((option) => ({
    key: option.value,
    label: option.label,
    leftIcon: <FontAwesomeIcon icon={option.icon} />,
    onSelect: () => handleHeadingChange(option.value),
  })), [handleHeadingChange]);
  const alignItems = useMemo(() => [
    {
      key: "align-left",
      label: "Izquierda",
      leftIcon: <FontAwesomeIcon icon={faAlignLeft} />,
      onSelect: () => applySavedTextAlign("left"),
    },
    {
      key: "align-center",
      label: "Centro",
      leftIcon: <FontAwesomeIcon icon={faAlignCenter} />,
      onSelect: () => applySavedTextAlign("center"),
    },
    {
      key: "align-right",
      label: "Derecha",
      leftIcon: <FontAwesomeIcon icon={faAlignRight} />,
      onSelect: () => applySavedTextAlign("right"),
    },
    {
      key: "align-justify",
      label: "Justificar",
      leftIcon: <FontAwesomeIcon icon={faAlignJustify} />,
      onSelect: () => applySavedTextAlign("justify"),
    },
  ], [applySavedTextAlign]);
  const structureItems = useMemo(() => [
    {
      key: "bullet-list",
      label: "Lista con vietas",
      leftIcon: <FontAwesomeIcon icon={faListUl} />,
      onSelect: () => runWithPreservedTextSelection((chain) => chain.toggleBulletList()),
    },
    {
      key: "ordered-list",
      label: "Lista numerada",
      leftIcon: <FontAwesomeIcon icon={faListOl} />,
      onSelect: () => runWithPreservedTextSelection((chain) => chain.toggleOrderedList()),
    },
    {
      key: "task-list",
      label: "Lista de tareas",
      leftIcon: <FontAwesomeIcon icon={faListCheck} />,
      onSelect: () => runWithPreservedTextSelection((chain) => chain.toggleTaskList()),
    },
  ], [runWithPreservedTextSelection]);
  const renderButtonGroup = useCallback((group: ToolbarButtonConfig[], label: string) => (
    <div className={styles.toolbarButtonGroup} role="group" aria-label={label}>
      {group.map((button) => renderActionButton(button))}
    </div>
  ), [renderActionButton]);

  return (
    <div
      className={styles.toolbar}
      role="toolbar"
      aria-label="Barra de herramientas del editor"
      data-toolbar-mode={isCompactToolbar ? "compact" : "default"}
      onMouseDownCapture={handleToolbarMouseDownCapture}
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
            open={isAlignDropdownOpen}
            onOpenChange={handleAlignDropdownOpenChange}
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
        {toolbarActions ? (
          <div className={styles.toolbarButtonGroup} role="group" aria-label="Acciones del editor">
            {toolbarActions}
          </div>
        ) : null}
      </div>
      {trailingContent ? (
        <div className={joinClasses(styles.toolbarSection, styles.toolbarSectionEnd)} data-group="view">
          {trailingContent}
        </div>
      ) : null}
    </div>
  );
}

export const AppEditorToolbar = memo(AppEditorToolbarComponent);
