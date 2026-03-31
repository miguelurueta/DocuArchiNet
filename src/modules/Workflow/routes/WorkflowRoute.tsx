import type { ReactNode } from "react";
import { Drawer } from "antd";
import { useNavigate } from "react-router-dom";
import Workflow from "../pages/Workflow";

interface WorkflowRouteProps {
  drawerContent?: ReactNode;
  drawerTitle?: string;
}

export default function WorkflowRoute({
  drawerContent,
  drawerTitle = "Detalle de workflow",
}: WorkflowRouteProps) {
  const navigate = useNavigate();
  const isDrawerOpen = Boolean(drawerContent);

  const handleClose = () => {
    navigate("/dashboard/workflow");
  };

  return (
    <>
      <Workflow />
      <Drawer
        title={drawerTitle}
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
