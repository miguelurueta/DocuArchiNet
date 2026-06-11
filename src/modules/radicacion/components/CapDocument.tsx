import { Suspense, lazy, useCallback, useMemo, useState } from "react";
import { Button, Dropdown, Layout } from "antd";
import {
  PrinterFilled,
  SaveFilled,
  InfoCircleFilled,
  ScanOutlined,
  LeftCircleFilled,
  RightCircleFilled,
  ZoomInOutlined,
  ZoomOutOutlined,
  DownCircleFilled,
  FileTextFilled,
  EditFilled,
  FolderOpenFilled,
  SwapOutlined,
  DeleteFilled,
  ReloadOutlined,
  StopFilled,
  DragOutlined,
  ExpandOutlined,
  StepBackwardFilled,
  StepForwardFilled,
  SearchOutlined,
  LinkOutlined,
  FileExcelFilled,
} from "@ant-design/icons";
import { AppTreeTable } from "../../../app/Components/UI/AppTreeTable";
import { AppDigitalizador } from "../../../app/Components/UI/AppDigitalizador";
import type { AppTreeTableRow } from "../../../app/Components/UI/AppTreeTable";
import { digitalizacionApiClient } from "../../digitalizacion/services/digitalizacionApi";
import type { DigitalizacionContext } from "../../digitalizacion/types/digitalizacion.types";
import styles from "../style/capdocument.module.css";

const { Content, Sider } = Layout;

const AppVisorEmbedPdfLazy = lazy(() =>
  import("../../../app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf").then((module) => ({
    default: module.AppVisorEmbedPdf,
  })),
);

type DocumentoRadicado = {
  id: string;
  nombre: string;
  tipo: string;
  estado: string;
  fileUrl?: string;
};

const documentosRadicado: DocumentoRadicado[] = [
  {
    id: "factura-1",
    nombre: "Factura.pdf",
    tipo: "PDF",
    estado: "Radicado",
  },
  {
    id: "factura-2",
    nombre: "Factura soporte.pdf",
    tipo: "PDF",
    estado: "Pendiente",
  },
  {
    id: "anexo-1",
    nombre: "Anexos",
    tipo: "Carpeta",
    estado: "2 documentos",
  },
];

const documentoTreeRows: AppTreeTableRow[] = documentosRadicado.map((documento) => ({
  id: documento.id,
  label: documento.nombre,
  values: {
    Documento: documento.nombre,
    Tipo: documento.tipo,
    Estado: documento.estado,
  },
}));

const CapDocument = () => {
  const [activeDocumentId, setActiveDocumentId] = useState<string | null>(null);

  const documentMenuItems = [
    {
      key: "1",
      icon: <FileTextFilled />,
      label: "Cambiar Tipologia",
    },
    {
      key: "2",
      icon: <EditFilled />,
      label: "Firma Digital",
    },
    {
      key: "3",
      icon: <FolderOpenFilled />,
      label: "Versiones del Documento",
    },
    {
      key: "4",
      icon: <SwapOutlined />,
      label: "Reemplazar Documento",
    },
    {
      type: "divider" as const,
    },
    {
      key: "5",
      icon: <DeleteFilled />,
      label: <span style={{ color: "red" }}>Eliminar</span>,
    },
  ];

  const printMenu = [
    { key: "1", label: "Imprimir documento actual" },
    { key: "2", label: "Imprimir todas las paginas" },
  ];

  const saveMenu = [
    { key: "1", label: "Guardar como PDF" },
    { key: "2", label: "Descargar original" },
  ];

  const zoomMenu = [
    { key: "1", label: "50%" },
    { key: "2", label: "75%" },
    { key: "3", label: "100%" },
    { key: "4", label: "150%" },
    { key: "5", label: "200%" },
  ];

  const linkMenu = [
    { key: "1", label: "Copiar enlace" },
    { key: "2", label: "Abrir en nueva pestana" },
  ];

  const activeDocument = useMemo(
    () => documentosRadicado.find((documento) => documento.id === activeDocumentId) ?? null,
    [activeDocumentId],
  );
  const digitalizacionContext = useMemo(
    () =>
      ({
        modo: "crear",
        nombreGabinete: "Radicacion",
        radicado: "RAD-2026",
        requiereMetadata: false,
        titulo: "Digitalizacion documental",
        sourceModule: "radicacion",
      }) satisfies DigitalizacionContext,
    [],
  );
  const handleDigitalizacionCompleted = useCallback(() => undefined, []);
  const handleDigitalizacionError = useCallback(() => undefined, []);
  const isPdfWorkspaceVisible = Boolean(activeDocument);

  return (
    <div className={styles.container}>
      <div className={styles.topToolbar}>
        <Button className={styles.btnAcept} icon={<PrinterFilled />}>
          Imprimir Rotulo
        </Button>
        <Button className={styles.btnAcept} icon={<SaveFilled />}>
          Guardar Rotulo
        </Button>
        <Button className={styles.btnAcept} icon={<InfoCircleFilled />}>
          Detalle Radicado
        </Button>
        <Button className={styles.btnAcept} icon={<ScanOutlined />}>
          Scanner
        </Button>
      </div>

      <div className={styles.viewerToolbar}>
        <Dropdown menu={{ items: printMenu }} trigger={["click"]}>
          <Button>
            <PrinterFilled />
            <DownCircleFilled style={{ fontSize: 10, marginLeft: 4 }} />
          </Button>
        </Dropdown>

        <Dropdown menu={{ items: saveMenu }} trigger={["click"]}>
          <Button>
            <SaveFilled />
            <DownCircleFilled style={{ fontSize: 10, marginLeft: 4 }} />
          </Button>
        </Dropdown>

        <Button icon={<ReloadOutlined />} />
        <Button
          icon={<StopFilled />}
          disabled={!activeDocument}
          onClick={() => setActiveDocumentId(null)}
        />
        <Button icon={<DragOutlined />} />
        <Button icon={<ExpandOutlined />} />

        <Button icon={<StepBackwardFilled />} />
        <Button icon={<LeftCircleFilled />} />
        <Button icon={<RightCircleFilled />} />
        <Button icon={<StepForwardFilled />} />

        <Button icon={<ZoomInOutlined />} />
        <Button icon={<ZoomOutOutlined />} />

        <Button icon={<FileExcelFilled />} />
        <Button icon={<SearchOutlined />} />

        <Dropdown menu={{ items: zoomMenu }} trigger={["click"]}>
          <Button>
            -1
            <DownCircleFilled style={{ fontSize: 10, marginLeft: 4 }} />
          </Button>
        </Dropdown>

        <Dropdown menu={{ items: linkMenu }} trigger={["click"]}>
          <Button>
            <LinkOutlined />
            <DownCircleFilled style={{ fontSize: 10, marginLeft: 4 }} />
          </Button>
        </Dropdown>
      </div>

      <Layout className={styles.mainLayout}>
        <Content className={styles.workspaceRegion}>
          <section
            className={styles.workspaceLayer}
            data-active={!isPdfWorkspaceVisible}
            aria-hidden={isPdfWorkspaceVisible}
            data-testid="digitalizacion-workspace"
          >
            <AppDigitalizador
              active={!isPdfWorkspaceVisible}
              context={digitalizacionContext}
              apiClient={digitalizacionApiClient}
              showFooterActions={false}
              onCompleted={handleDigitalizacionCompleted}
              onError={handleDigitalizacionError}
            />
          </section>

          <section
            className={styles.workspaceLayer}
            data-active={isPdfWorkspaceVisible}
            aria-hidden={!isPdfWorkspaceVisible}
            data-testid="pdf-viewer-workspace"
          >
            <div className={styles.pdfWorkspace}>
              <div className={styles.pdfHeader}>
                <div>
                  <h3 className={styles.sectionTitle}>{activeDocument?.nombre ?? "Documento PDF"}</h3>
                  <p className={styles.subtitle}>Visor persistente sobre el mismo espacio del workspace.</p>
                </div>
                <Button size="small" onClick={() => setActiveDocumentId(null)}>
                  Cerrar visor
                </Button>
              </div>
              <div className={styles.pdfViewerFrame}>
                {activeDocument ? (
                  <Suspense fallback={<div className={styles.pdfEmptyState}>Cargando visor PDF...</div>}>
                    <AppVisorEmbedPdfLazy fileUrl={activeDocument.fileUrl} />
                  </Suspense>
                ) : (
                  <div className={styles.pdfEmptyState}>Seleccione un documento para abrir el visor.</div>
                )}
              </div>
            </div>
          </section>
        </Content>

        <Sider width="30%" className={styles.rightPanel}>
          <div className={styles.docHeader}>
            <div className={styles.docHeaderLeft}>
              <span>Documentos: {documentosRadicado.length}</span>
            </div>

            <div className={styles.docHeaderActions}>
              <Button type="text" size="small" icon={<InfoCircleFilled />} />
              <Button type="text" size="small" icon={<DeleteFilled />} />
              <Button type="text" size="small" icon={<EditFilled />} />
              <Dropdown menu={{ items: documentMenuItems }} trigger={["click"]} placement="bottomRight">
                <Button type="text" size="small" icon={<DownCircleFilled />} />
              </Dropdown>
            </div>
          </div>

          <AppTreeTable
            rows={documentoTreeRows}
            columns={["Documento", "Tipo", "Estado"]}
            activeRowId={activeDocumentId ?? undefined}
            rowClickAffordance
            rowClickTooltip="Abrir documento"
            tableLayoutMode="fill"
            tableDomLayout="normal"
            onSelectRow={setActiveDocumentId}
          />
        </Sider>
      </Layout>
    </div>
  );
};

export default CapDocument;
