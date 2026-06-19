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
    setIsAssistantClosing(false);
    setIsAssistantOpen(true);
  };

  const closeAssistant = () => {
    setIsAssistantClosing(true);
    window.setTimeout(() => {
      setIsAssistantOpen(false);
      setIsAssistantClosing(false);
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
          </div>
        </div>
      </GestionRespuestaDocumentosProvider>
    </div>
  );
}
