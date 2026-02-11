import { Layout, Button, Avatar, Dropdown, Space, Typography } from "antd";
import {
  MenuFoldOutlined,
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
  onToggle: () => void;
}

const profileMenu = {
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
      type: "divider",
    },
    {
      key: "logout",
      icon: <LogoutOutlined />,
      label: "Cerrar sesión",
      danger: true,
    },
  ],
};

const Navbar = ({ collapsed, onToggle }: NavbarProps) => {
  const navigate = useNavigate();

  return (
    <Header className={styles.header}>
      {/* LEFT */}
      <div className={styles.left}>
        <Button
          type="text"
          icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
          onClick={onToggle}
          className={styles.trigger}
          aria-label="Colapsar menú"
        />

        {/* HOME – SPA */}
        <Button
          type="text"
          icon={<HomeOutlined />}
          onClick={() => navigate("/dashboard")}
          className={styles.trigger}
          aria-label="Ir al dashboard"
        />
      </div>

      {/* RIGHT */}
      <div className={styles.right}>
        <Dropdown menu={profileMenu} trigger={["click"]}>
          <Space className={styles.profile}>
            <Avatar size="medium" icon={<UserOutlined />} />
            <Text className={styles.username}>Miguel Urueta</Text>
          </Space>
        </Dropdown>
      </div>
    </Header>
  );
};

export default Navbar;
