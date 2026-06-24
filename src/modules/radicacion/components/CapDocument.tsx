import { FileTextOutlined, PrinterOutlined, SaveOutlined, ScanOutlined } from "@ant-design/icons";
import { useCallback, useState } from "react";
import { AppButton } from "../../../app/Components/UI/AppButton";
import { AppCollapseRail } from "../../../app/Components/UI/AppCollapseRail";
import { AppDigitalizador } from "../../../app/Components/UI/AppDigitalizador";
import { AppTreeTable } from "../../../app/Components/UI/AppTreeTable";
import type {
  DigitalizacionContext,
  DigitalizacionDocumentalError,
  DigitalizacionResult,
} from "../../../modules/digitalizacion";
import type { AppTreeTableRow } from "../../../app/Components/UI/AppTreeTable";
import styles from "../style/capdocument.module.css";

const CAPTURA_ARBOL_FILAS: AppTreeTableRow[] = [
  { id: "documentos-recientes", label: "Documentos recientes" },
  { id: "radicados", label: "Radicados" },
  { id: "plantillas", label: "Plantillas" },
];

const CAPTURA_RADICACION_CONTEXT: DigitalizacionContext = {
  modo: "crear",
  nombreGabinete: "Radicacion",
  sourceModule: "Radicacion",
};

const CapDocument = () => {
  const [treeCollapsed, setTreeCollapsed] = useState(false);
  const [activeTreeRowId, setActiveTreeRowId] = useState<string | undefined>(
    CAPTURA_ARBOL_FILAS[0]?.id,
  );

  const handleCompleted = useCallback((result: DigitalizacionResult) => {
    void result;
  }, []);

  const handleError = useCallback((_error: DigitalizacionDocumentalError) => {
    void _error;
  }, []);

  const handleTreeToggle = useCallback(() => {
    setTreeCollapsed((current) => !current);
  }, []);

  const handleTreeRowSelect = useCallback((rowId: string) => {
    setActiveTreeRowId(rowId);
  }, []);

  return (
    <div className={styles.container}>
      <div className={styles.radicacionToolbar} role="toolbar" aria-label="Acciones de radicación">
        <AppButton icon={<PrinterOutlined />} onClick={() => void 0}>
          Imprimir Rótulo
        </AppButton>
        <AppButton icon={<SaveOutlined />} onClick={() => void 0}>
          Guardar Rótulo
        </AppButton>
        <AppButton icon={<FileTextOutlined />} onClick={() => void 0}>
          Detalle Radicado
        </AppButton>
        <AppButton icon={<ScanOutlined />} onClick={() => void 0}>
          Scanner
        </AppButton>
      </div>

      <div className={styles.workspaceHost}>
        <AppCollapseRail
          title="Documentos"
          collapsed={treeCollapsed}
          onToggle={handleTreeToggle}
          variant="overlay"
          placement="left"
          railLabel="Documentos"
          railIcon={<FileTextOutlined />}
          className={styles.documentTree}
          panelId="radicacion-tree-panel"
          hideHeader={false}
        >
          <div className={styles.treeSurface}>
            <AppTreeTable
              rows={CAPTURA_ARBOL_FILAS}
              activeRowId={activeTreeRowId}
              onSelectRow={handleTreeRowSelect}
              rowClickTooltip="Abrir"
              rowSelection="single"
              onSelectionChanged={(selectedRows) => {
                if (selectedRows[0]) {
                  setActiveTreeRowId(selectedRows[0]);
                }
              }}
            />
          </div>
        </AppCollapseRail>

        <section className={styles.digitalizadorShell}>
          <AppDigitalizador
            context={CAPTURA_RADICACION_CONTEXT}
            modulo="Radicacion"
            licenciaDynamsoft={import.meta.env.VITE_DYNAMSOFT_LICENSE_KEY}
            onCompleted={handleCompleted}
            onError={handleError}
            active
            showHeader={false}
            showWorkspaceSummary={false}
            showWorkspaceState={false}
            showLegacyFooter={false}
          />
        </section>
      </div>
    </div>
  );
};

export default CapDocument;
