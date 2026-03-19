import React from "react";
import { Tabs, Space } from "antd";
import {
  FileAddFilled,
  FileTextFilled,
  SettingFilled,
  OpenAIFilled,
} from "@ant-design/icons";

import styles from "../style/tabs.module.css";
import CapDocument from "../components/CapDocument";
import RadicacionForm from "../components/RadicacionForm";
import ModalPendiente from "../components/Modalpendiente";

const onChange = (key: string) => {
  console.log(key);
};

const TabsDocu: React.FC = () => {
  const items = [
    {
      key: "1",
      label: (
        <Space>
          <OpenAIFilled />
          IA
        </Space>
      ),
      children: "Contenido de IA",
    },
    {
      key: "2",
      label: (
        <Space>
          <FileAddFilled />
          Radicación
        </Space>
      ),
      children: <RadicacionForm/>,
    },
    {
      key: "3",
      label: (
        <Space>
          <FileTextFilled />
          Captura de Documentos
        </Space>
      ),
      children: <CapDocument />,
    },
    {
      key: "4",
      label: (
        <Space>
          <SettingFilled />
          Gestión de Radicados
        </Space>
      ),
      children: <div>Contenido de Configuración</div>,
    },
  ];

  return (
    <div>
      <Tabs
        type="card"
        onChange={onChange}
        items={items}
        className={styles.customTabs}
        tabBarExtraContent={{
          right: <ModalPendiente />,
        }}
      />
    </div>
  );
};
export default TabsDocu;


