import React, { useEffect, useState } from "react";
import { Space, Tabs } from "antd";
import {
  FileAddFilled,
  FileTextFilled,
  OpenAIFilled,
  SettingFilled,
} from "@ant-design/icons";

import styles from "../style/tabs.module.css";
import CapDocument from "../components/CapDocument";
import RadicacionForm from "../components/RadicacionForm";
import ModalPendiente from "../components/Modalpendiente";
import { EnviarPendienteConfirmModal } from "../components/EnviarPendienteConfirmModal";
import { RadicacionDocumentosGuard } from "../components/RadicacionDocumentosGuard";
import { useRadicacionDocumentalContext } from "./useRadicacionDocumentalContext";
import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";
import type { PlantillaRadicadoDTO } from "../models/PlantillaRadicadoDTO";
import {
  RADICACION_TAB_KEYS,
  type RadicacionTabKey,
  resolveRadicacionTabFromDestino,
} from "../routes/radicacionRoutes";

interface TabsDocuProps {
  plantilla: PlantillaRadicadoDTO;
  camposPlantilla: ReadonlyArray<CampoPlantillaDTO>;
}

const TabsDocu: React.FC<TabsDocuProps> = ({
  plantilla,
  camposPlantilla,
}) => {
  const { destinoPostRegistro, tieneTramiteDocumentalActivoEstado0 } =
    useRadicacionDocumentalContext();
  const resolvedInitialTab = resolveRadicacionTabFromDestino({
    destinoPostRegistro,
    documentosDisponibles: tieneTramiteDocumentalActivoEstado0,
  });
  const [activeKey, setActiveKey] = useState(resolvedInitialTab);

  useEffect(() => {
    setActiveKey(resolvedInitialTab);
  }, [resolvedInitialTab]);

  const handleTabChange = (nextKey: string) => {
    if (
      Object.values(RADICACION_TAB_KEYS).includes(nextKey as RadicacionTabKey)
    ) {
      setActiveKey(nextKey as RadicacionTabKey);
    }
  };

  const items = [
    {
      key: RADICACION_TAB_KEYS.ia,
      label: (
        <Space>
          <OpenAIFilled />
          IA
        </Space>
      ),
      children: "Asistencia IA no disponible en esta fase.",
    },
    {
      key: RADICACION_TAB_KEYS.radicacion,
      label: (
        <Space>
          <FileAddFilled />
          Radicación
        </Space>
      ),
      children: (
        <RadicacionForm
          plantilla={plantilla}
          camposPlantilla={camposPlantilla}
        />
      ),
    },
    {
      key: RADICACION_TAB_KEYS.documentos,
      label: (
        <Space>
          <FileTextFilled />
          Captura de Documentos
        </Space>
      ),
      disabled: !tieneTramiteDocumentalActivoEstado0,
      children: (
        <RadicacionDocumentosGuard>
          <CapDocument />
        </RadicacionDocumentosGuard>
      ),
    },
    {
      key: RADICACION_TAB_KEYS.gestionRadicados,
      label: (
        <Space>
          <SettingFilled />
          Gestión de Radicados
        </Space>
      ),
      children: <div>Gestión de radicados no disponible en esta fase.</div>,
    },
  ];

  return (
    <div>
      <Tabs
        type="card"
        activeKey={activeKey}
        onChange={handleTabChange}
        items={items}
        className={styles.customTabs}
        tabBarExtraContent={{
          right: (
            <Space>
              <EnviarPendienteConfirmModal />
              <ModalPendiente />
            </Space>
          ),
        }}
      />
    </div>
  );
};

export default TabsDocu;
