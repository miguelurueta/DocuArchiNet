import {
  useCallback,
  useEffect,
  useRef,
  useState,
  type KeyboardEvent,
} from "react";
import {
  DoubleLeftOutlined,
  DoubleRightOutlined,
  LeftOutlined,
  RightOutlined,
} from "@ant-design/icons";
import { AppButton } from "../../../../app/Components/UI/AppButton";
import styles from "./DigitalizacionDocumentalWorkspace.module.css";

type PageNavigatorFloatingProps = {
  currentPage: number;
  totalPages: number;
  onFirstPage: () => void;
  onPreviousPage: () => void;
  onNextPage: () => void;
  onLastPage: () => void;
  onGoToPage: (pageNumber: number) => void;
};

const AUTOHIDE_DELAY_MS = 3000;

const isEditableTarget = (target: EventTarget | null) => {
  if (!(target instanceof HTMLElement)) {
    return false;
  }

  const tagName = target.tagName.toLowerCase();
  return (
    tagName === "input" ||
    tagName === "textarea" ||
    tagName === "select" ||
    target.isContentEditable
  );
};

export function PageNavigatorFloating({
  currentPage,
  totalPages,
  onFirstPage,
  onPreviousPage,
  onNextPage,
  onLastPage,
  onGoToPage,
}: PageNavigatorFloatingProps) {
  const [editing, setEditing] = useState(false);
  const [draftValue, setDraftValue] = useState(String(currentPage || 1));
  const [dimmed, setDimmed] = useState(false);
  const inputRef = useRef<HTMLInputElement | null>(null);

  const resetAutohide = useCallback(() => {
    setDimmed(false);
  }, []);

  useEffect(() => {
    if (!editing) {
      return undefined;
    }

    inputRef.current?.focus();
    inputRef.current?.select();
    return undefined;
  }, [editing]);

  useEffect(() => {
    if (totalPages <= 0 || editing) {
      return undefined;
    }

    const timeoutId = window.setTimeout(() => {
      setDimmed(true);
    }, AUTOHIDE_DELAY_MS);

    return () => {
      window.clearTimeout(timeoutId);
    };
  }, [currentPage, editing, totalPages]);

  useEffect(() => {
    if (totalPages <= 0) {
      return undefined;
    }

    const handleKeyDown = (event: globalThis.KeyboardEvent) => {
      if (isEditableTarget(event.target)) {
        return;
      }

      if (event.altKey || event.ctrlKey || event.metaKey || event.shiftKey) {
        return;
      }

      if (event.key === "ArrowLeft") {
        event.preventDefault();
        resetAutohide();
        onPreviousPage();
      } else if (event.key === "ArrowRight") {
        event.preventDefault();
        resetAutohide();
        onNextPage();
      } else if (event.key === "Home") {
        event.preventDefault();
        resetAutohide();
        onFirstPage();
      } else if (event.key === "End") {
        event.preventDefault();
        resetAutohide();
        onLastPage();
      }
    };

    document.addEventListener("keydown", handleKeyDown);
    return () => {
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [onFirstPage, onLastPage, onNextPage, onPreviousPage, resetAutohide, totalPages]);

  if (totalPages <= 0) {
    return null;
  }

  const commitDraft = () => {
    const requestedPage = Number.parseInt(draftValue, 10);
    setEditing(false);
    if (Number.isInteger(requestedPage)) {
      onGoToPage(requestedPage);
    }
  };

  const handleInputKeyDown = (event: KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter") {
      event.preventDefault();
      commitDraft();
    } else if (event.key === "Escape") {
      event.preventDefault();
      setEditing(false);
      setDraftValue(String(currentPage || 1));
    }
  };

  return (
    <div
      className={styles.pageNavigatorFloating}
      data-dimmed={dimmed ? "true" : "false"}
      aria-label="Navegacion de paginas"
      onMouseMove={resetAutohide}
      onFocus={resetAutohide}
    >
      <AppButton
        variant="ghost"
        size="sm"
        icon={<DoubleLeftOutlined />}
        aria-label="Primera pagina"
        tooltip="Primera pagina"
        onClick={onFirstPage}
        disabled={currentPage <= 1}
      />
      <AppButton
        variant="ghost"
        size="sm"
        icon={<LeftOutlined />}
        aria-label="Pagina anterior"
        tooltip="Pagina anterior"
        onClick={onPreviousPage}
        disabled={currentPage <= 1}
      />
      <span className={styles.pageNavigatorLabel}>
        Pagina{" "}
        {editing ? (
          <input
            ref={inputRef}
            className={styles.pageNavigatorInput}
            type="number"
            min={1}
            max={totalPages}
            value={draftValue}
            onChange={(event) => setDraftValue(event.target.value)}
            onKeyDown={handleInputKeyDown}
            onBlur={() => {
              setEditing(false);
              setDraftValue(String(currentPage || 1));
            }}
            aria-label="Pagina destino"
          />
        ) : (
          <button
            className={styles.pageNavigatorCurrent}
            type="button"
            onClick={() => {
              setDraftValue(String(currentPage || 1));
              setEditing(true);
            }}
            aria-label="Editar pagina actual"
          >
            {currentPage}
          </button>
        )}{" "}
        de {totalPages}
      </span>
      <AppButton
        variant="ghost"
        size="sm"
        icon={<RightOutlined />}
        aria-label="Pagina siguiente"
        tooltip="Pagina siguiente"
        onClick={onNextPage}
        disabled={currentPage >= totalPages}
      />
      <AppButton
        variant="ghost"
        size="sm"
        icon={<DoubleRightOutlined />}
        aria-label="Ultima pagina"
        tooltip="Ultima pagina"
        onClick={onLastPage}
        disabled={currentPage >= totalPages}
      />
    </div>
  );
}
