import { Layout, Button, Avatar, Dropdown, Space, Typography } from "antd";
import type { MenuProps } from "antd";
import {
  MenuFoldOutlined,
  MenuOutlined,
  MenuUnfoldOutlined,
  UserOutlined,
  LogoutOutlined,
  SettingOutlined,
  HomeOutlined,
} from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import styles from "../style/navbar.module.css";

const { Header } = Layout;
const { Text } = Typography;

interface NavbarProps {
  collapsed: boolean;
  isMobile: boolean;
  onToggle: () => void;
}

const profileMenu: MenuProps = {
  items: [
    {
      key: "profile",
      icon: <UserOutlined />,
      label: "Mi perfil",
    },
    {
      key: "settings",
      icon: <SettingOutlined />,
      label: "Configuración",
    },
    {
      type: "divider" as const,
    },
    {
      key: "logout",
      icon: <LogoutOutlined />,
      label: "Cerrar sesión",
      danger: true,
    },
  ],
};

const Navbar = ({ collapsed, isMobile, onToggle }: NavbarProps) => {
  const navigate = useNavigate();

  return (
    <Header className={styles.header}>
      <div className={styles.left}>
        <Button
          type="text"
          icon={
            isMobile
              ? <MenuOutlined />
              : collapsed
                ? <MenuUnfoldOutlined />
                : <MenuFoldOutlined />
          }
          onClick={onToggle}
          className={styles.trigger}
          aria-label={isMobile ? "Abrir menú" : collapsed ? "Expandir menú" : "Colapsar menú"}
        />

        <Button
          type="text"
          icon={<HomeOutlined />}
          onClick={() => navigate("/dashboard")}
          className={styles.trigger}
          aria-label="Ir al dashboard"
        />
      </div>

      <div className={styles.right}>
        <Dropdown menu={profileMenu} trigger={["click"]}>
          <Space className={styles.profile}>
            <Avatar size="default" icon={<UserOutlined />} />
            <Text className={styles.username}>Miguel Urueta</Text>
          </Space>
        </Dropdown>
      </div>
    </Header>
  );
};

export default Navbar;
