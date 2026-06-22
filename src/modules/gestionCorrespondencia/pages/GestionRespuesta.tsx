import {
  CloseOutlined,
  FileTextOutlined,
  InfoCircleOutlined,
  RobotOutlined,
  SendOutlined,
} from "@ant-design/icons";
import { Switch } from "antd";
import type { KeyboardEvent } from "react";
import { useEffect, useRef, useState } from "react";
import { useParams } from "react-router-dom";
import type { AppTabItem } from "../../../app/Components/UI/AppTabs";
import { AppTabs } from "../../../app/Components/UI/AppTabs";
import { DocumentosWorkbench } from "../components/documentosWorkbench";
import { GestionWorkbenchParallelTabs } from "../components/workbenchParallelTabs";
import { GestionRespuestaDocumentosProvider } from "../context/GestionRespuestaDocumentosContext";
import { GestionRespuestaMainTabContent } from "../components/gestionRespuestaMainTab/GestionRespuestaMainTabContent";
import styles from "../style/GestionRespuesta.module.css";

type GestionWorkbenchLayoutMode = "tabs" | "parallel";

const PARALLEL_LAYOUT_QUERY = "(min-width: 901px)";
const ASSISTANT_RESPONSE_SUGGESTIONS = [
  "Redacta una respuesta formal para este tramite.",
  "Resume el contexto antes de responder.",
  "Propone una respuesta breve y clara.",
];

function useCanUseParallelLayout() {
  const [canUseParallelLayout, setCanUseParallelLayout] = useState(() =>
    typeof window === "undefined" || typeof window.matchMedia !== "function"
      ? true
      : window.matchMedia(PARALLEL_LAYOUT_QUERY).matches,
  );

  useEffect(() => {
    if (typeof window === "undefined" || typeof window.matchMedia !== "function") return;
    const mediaQueryList = window.matchMedia(PARALLEL_LAYOUT_QUERY);
    const update = (event: MediaQueryListEvent) => setCanUseParallelLayout(event.matches);

    setCanUseParallelLayout(mediaQueryList.matches);
    mediaQueryList.addEventListener("change", update);
    return () => mediaQueryList.removeEventListener("change", update);
  }, []);

  return canUseParallelLayout;
}

type GestionRespuestaProps = {
  idTareaWf?: number;
  radicado?: string;
  idRespuestaRadicado?: string | number;
  detailState?: "loading" | "ready" | "blocked-empty" | "blocked-error" | "blocked-invalid-id";
};

export default function GestionRespuesta({
  idTareaWf: idTareaWfFromRoute,
  radicado,
  idRespuestaRadicado,
}: GestionRespuestaProps = {}) {
  const params = useParams();
  const canUseParallelLayout = useCanUseParallelLayout();
  const [layoutMode, setLayoutMode] = useState<GestionWorkbenchLayoutMode>("tabs");
  const [isAssistantOpen, setIsAssistantOpen] = useState(false);
  const [isAssistantClosing, setIsAssistantClosing] = useState(false);
  const assistantInputRef = useRef<HTMLInputElement | null>(null);
  const assistantCloseTimeoutRef = useRef<number | null>(null);
  const [assistantMessages, setAssistantMessages] = useState<
    Array<{ id: string; role: "assistant" | "user"; text: string }>
  >([
    {
      id: "assistant-welcome",
      role: "assistant",
      text: "Asistente listo para apoyar la gestion del tramite.",
    },
  ]);
  const rawId = params.id;
  const fallbackId = typeof rawId === "string" ? Number.parseInt(rawId, 10) : Number.NaN;
  const idTareaWf =
    typeof idTareaWfFromRoute === "number" && Number.isFinite(idTareaWfFromRoute)
      ? idTareaWfFromRoute
      : fallbackId;
  const resolvedIdTareaWf = Number.isFinite(idTareaWf) ? idTareaWf : undefined;
  const isParallel = layoutMode === "parallel" && canUseParallelLayout;

  useEffect(() => {
    if (!canUseParallelLayout && layoutMode === "parallel") {
      setLayoutMode("tabs");
    }
  }, [canUseParallelLayout, layoutMode]);

  useEffect(
    () => () => {
      if (assistantCloseTimeoutRef.current !== null) {
        window.clearTimeout(assistantCloseTimeoutRef.current);
      }
    },
    [],
  );

  const sendAssistantMessage = () => {
    const trimmedDraft = assistantInputRef.current?.value.trim() ?? "";
    if (!trimmedDraft) return;

    setAssistantMessages((currentMessages) => [
      ...currentMessages,
      {
        id: `user-${Date.now()}`,
        role: "user",
        text: trimmedDraft,
      },
      {
        id: `assistant-${Date.now()}`,
        role: "assistant",
        text: "Mensaje recibido. La integracion del servicio conversacional queda lista para conectarse aqui.",
      },
    ]);
    if (assistantInputRef.current) {
      assistantInputRef.current.value = "";
      assistantInputRef.current.focus();
    }
  };

  const openAssistant = () => {
    if (assistantCloseTimeoutRef.current !== null) {
      window.clearTimeout(assistantCloseTimeoutRef.current);
      assistantCloseTimeoutRef.current = null;
    }
    setIsAssistantClosing(false);
    setIsAssistantOpen(true);
  };

  const closeAssistant = () => {
    assistantInputRef.current?.blur();
    if (assistantCloseTimeoutRef.current !== null) {
      window.clearTimeout(assistantCloseTimeoutRef.current);
    }
    setIsAssistantClosing(true);
    assistantCloseTimeoutRef.current = window.setTimeout(() => {
      setIsAssistantOpen(false);
      setIsAssistantClosing(false);
      assistantCloseTimeoutRef.current = null;
    }, 190);
  };

  const toggleAssistant = () => {
    if (isAssistantOpen) {
      closeAssistant();
      return;
    }

    openAssistant();
  };

  const handleAssistantKeyDownCapture = (event: KeyboardEvent<HTMLElement>) => {
    if (event.target === assistantInputRef.current && event.key === "Enter" && !event.shiftKey) {
      event.preventDefault();
      sendAssistantMessage();
    }

    event.stopPropagation();
  };

  const applyAssistantSuggestion = (suggestion: string) => {
    if (!assistantInputRef.current) return;
    assistantInputRef.current.value = suggestion;
    assistantInputRef.current.focus();
  };

  const gestionContent = <GestionRespuestaMainTabContent idTareaWf={resolvedIdTareaWf} />;
  const documentosContent = <DocumentosWorkbench idTareaWf={resolvedIdTareaWf} />;
  const layoutToggleButton = (
    <label
      className={styles.layoutSwitchControl}
      data-layout-state={isParallel ? "active" : "inactive"}
      title={
        canUseParallelLayout
          ? undefined
          : "La vista paralela esta disponible en pantallas mas anchas."
      }
    >
      <span className={styles.layoutSwitchText}>Vista paralela</span>
      <Switch
        checked={isParallel}
        disabled={!canUseParallelLayout}
        aria-label="Vista paralela"
        aria-pressed={isParallel}
        onChange={(checked) => setLayoutMode(checked ? "parallel" : "tabs")}
      />
    </label>
  );

  const items: AppTabItem[] = [
    {
      key: "gestion",
      label: "Gestion",
      icon: <InfoCircleOutlined />,
      children: gestionContent,
    },
    {
      key: "documentos",
      label: "Documentos",
      icon: <FileTextOutlined />,
      children: documentosContent,
    },
  ];

  return (
    <div className={styles.tabsShell}>
      <GestionRespuestaDocumentosProvider
        idTareaWf={resolvedIdTareaWf}
        radicado={radicado}
        idRespuestaRadicado={idRespuestaRadicado}
      >
        <div className={styles.layoutBody}>
          {isParallel ? (
            <>
              <div className={styles.parallelTabsNav} role="tablist" aria-label="Vista paralela">
                <div className={styles.parallelTabsList}>
                  <span className={styles.parallelTabItem}>
                    <InfoCircleOutlined />
                    <span>Gestion</span>
                  </span>
                  <span className={styles.parallelTabItem}>
                    <FileTextOutlined />
                    <span>Documentos</span>
                  </span>
                </div>
                <div className={styles.parallelTabsExtra}>{layoutToggleButton}</div>
              </div>
              <GestionWorkbenchParallelTabs
                gestion={gestionContent}
                documentos={documentosContent}
              />
            </>
          ) : (
            <AppTabs
              items={items}
              fullWidth
              className={styles.tabs}
              tabBarExtraContent={{ right: layoutToggleButton }}
            />
          )}
          <div className={styles.workbenchAssistantLayer}>
            {isAssistantOpen ? (
              <section
                className={styles.assistantPanel}
                data-closing={isAssistantClosing ? "true" : "false"}
                aria-label="Chat de asistencia del workbench"
                onKeyDownCapture={handleAssistantKeyDownCapture}
                onKeyUpCapture={(event) => event.stopPropagation()}
                onPointerDownCapture={(event) => event.stopPropagation()}
              >
                <header className={styles.assistantHeader}>
                  <span className={styles.assistantTitle}>
                    <RobotOutlined />
                    <span>IA</span>
                  </span>
                  <button
                    type="button"
                    className={styles.assistantCloseButton}
                    onClick={closeAssistant}
                    aria-label="Cerrar asistente"
                  >
                    <CloseOutlined />
                  </button>
                </header>
                <div className={styles.assistantMessages} role="log" aria-live="polite">
                  {assistantMessages.map((message) => (
                    <div
                      key={message.id}
                      className={styles.assistantMessage}
                      data-role={message.role}
                    >
                      {message.text}
                    </div>
                  ))}
                </div>
                <div className={styles.assistantSuggestions} aria-label="Sugerencias de respuesta">
                  <span className={styles.assistantSuggestionsTitle}>Sugerencias</span>
                  <div className={styles.assistantSuggestionList}>
                    {ASSISTANT_RESPONSE_SUGGESTIONS.map((suggestion) => (
                      <button
                        key={suggestion}
                        type="button"
                        className={styles.assistantSuggestion}
                        onClick={() => applyAssistantSuggestion(suggestion)}
                      >
                        {suggestion}
                      </button>
                    ))}
                  </div>
                </div>
                <form
                  className={styles.assistantComposer}
                  onSubmit={(event) => {
                    event.preventDefault();
                    sendAssistantMessage();
                  }}
                >
                  <span className={styles.assistantInputShell}>
                    <input
                      ref={assistantInputRef}
                      className={styles.assistantInput}
                      type="text"
                      onKeyUp={(event) => event.stopPropagation()}
                      onClick={(event) => event.stopPropagation()}
                      placeholder="Escribe una pregunta..."
                      aria-label="Mensaje para el asistente"
                    />
                    <button
                      type="button"
                      className={styles.assistantClearButton}
                      onClick={(event) => {
                        event.stopPropagation();
                        if (assistantInputRef.current) {
                          assistantInputRef.current.value = "";
                          assistantInputRef.current.focus();
                        }
                      }}
                      aria-label="Limpiar mensaje"
                    >
                      <CloseOutlined />
                    </button>
                  </span>
                  <button
                    type="submit"
                    className={styles.assistantSendButton}
                    aria-label="Enviar mensaje"
                  >
                    <SendOutlined />
                  </button>
                </form>
              </section>
            ) : null}
            <button
              type="button"
              className={styles.assistantFab}
              data-open={isAssistantOpen ? "true" : "false"}
              onClick={toggleAssistant}
              aria-label={isAssistantOpen ? "Cerrar asistente" : "Abrir asistente"}
              aria-expanded={isAssistantOpen}
            >
              {isAssistantOpen ? <CloseOutlined /> : <RobotOutlined />}
              <span className={styles.assistantFabLabel}>IA</span>
            </button>
            {!isAssistantOpen ? (
              <button
                type="button"
                className={styles.assistantHint}
                onClick={openAssistant}
                aria-label="Abrir asistente para generar la respuesta"
              >
                ¿Te ayudo con la respuesta?
              </button>
            ) : null}
          </div>
        </div>
      </GestionRespuestaDocumentosProvider>
    </div>
  );
}
