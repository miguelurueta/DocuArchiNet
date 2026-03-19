import { useState } from "react";
import { Modal, Button, Table } from "antd";
import type { ColumnsType } from "antd/es/table";
import { FileOutlined } from "@ant-design/icons";
import styles from "../style/botonacept.module.css";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faClipboardList } from "@fortawesome/free-solid-svg-icons";

interface Radicado {
  key: string;
  numero: string;
  remitente: string;
  tramite: string;
  fecha: string;
}

const data: Radicado[] = [
  {
    key: "1",
    numero: "25000270980",
    remitente: "Juan Pérez",
    tramite: "Licencia de construcción",
    fecha: "27/10/2025",
  },
  {
    key: "2",
    numero: "25000270981",
    remitente: "María Gómez",
    tramite: "Solicitud de concepto",
    fecha: "25/10/2025",
  },
  {
    key: "3",
    numero: "25000270982",
    remitente: "Carlos Ruiz",
    tramite: "Radicación de planos",
    fecha: "23/10/2025",
  },
];

export default function ModalPendiente() {
  const [open, setOpen] = useState(false);

  const columns: ColumnsType<Radicado> = [
    {
      title: "NÚMERO RADICADO",
      dataIndex: "numero",
      key: "numero",
    },
    {
      title: "REMITENTE",
      dataIndex: "remitente",
      key: "remitente",
    },
    {
      title: "TRÁMITE",
      dataIndex: "tramite",
      key: "tramite",
    },
    {
      title: "FECHA",
      dataIndex: "fecha",
      key: "fecha",
    },
    {
      title: "OPCIONES",
      key: "opciones",
      render: () => (
        <Button
          type="primary"
          icon={<FileOutlined />}
          size="small"
        />
      ),
    },
  ];

  return (
    <>
      {/* Botón que abre el modal */}
      <Button type="primary" size="large" className={styles.btnAcept} onClick={() => setOpen(true)}>
        <FontAwesomeIcon icon={faClipboardList} />
        Pendientes
      </Button>

      {/* Modal */}
      <Modal
        title="Listado de Radicados"
        open={open}
        onCancel={() => setOpen(false)}
        footer={[
          <Button key="close" type="primary" onClick={() => setOpen(false)}>
            Cerrar
          </Button>,
        ]}
        width={900}
      >
        <Table
          columns={columns}
          dataSource={data}
          pagination={{ pageSize: 3 }}
        />
      </Modal>
    </>
  );
}
