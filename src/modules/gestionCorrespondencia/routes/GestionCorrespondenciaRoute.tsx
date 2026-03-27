import type { ReactNode } from "react";
import { Drawer } from "antd";
import { useNavigate } from "react-router-dom";
import GestionCorrespondencia from "../pages/GestionCorrespondencia";

interface GestionCorrespondenciaRouteProps {
  drawerContent?: ReactNode;
}

export default function GestionCorrespondenciaRoute({
  drawerContent,
}: GestionCorrespondenciaRouteProps) {
  const navigate = useNavigate();
  const isDrawerOpen = Boolean(drawerContent);

  const handleClose = () => {
    navigate("/dashboard/gestion-correspondencia");
  };

  return (
    <>
      <GestionCorrespondencia />
      <Drawer
        title="Respuesta contextual"
        placement="right"
        size="large"
        open={isDrawerOpen}
        onClose={handleClose}
        destroyOnClose
        getContainer={false}
        maskClosable
      >
        {drawerContent}
      </Drawer>
    </>
  );
}
