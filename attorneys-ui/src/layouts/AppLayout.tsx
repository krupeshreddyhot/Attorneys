import {

  AppBar,

  Box,

  Button,

  CssBaseline,

  Drawer,

  IconButton,

  List,

  ListItemButton,

  ListItemIcon,

  ListItemText,

  Toolbar,

  Typography,

} from "@mui/material";

import MenuIcon from "@mui/icons-material/Menu";

import DashboardIcon from "@mui/icons-material/Dashboard";

import GavelIcon from "@mui/icons-material/Gavel";

import AccountBalanceIcon from "@mui/icons-material/AccountBalance";

import FolderIcon from "@mui/icons-material/Folder";

import AssessmentIcon from "@mui/icons-material/Assessment";

import DescriptionIcon from "@mui/icons-material/Description";

import LanguageIcon from "@mui/icons-material/Language";

import PaymentsIcon from "@mui/icons-material/Payments";

import BusinessIcon from "@mui/icons-material/Business";

import { useState } from "react";

import { Outlet, useLocation, useNavigate, Link as RouterLink } from "react-router-dom";

import { useAuth } from "../context/AuthContext";



const drawerWidth = 260;



type MenuItem = { text: string; path: string; icon: React.ReactNode; adminOnly?: boolean };



const menuItems: MenuItem[] = [

  { text: "Dashboard", path: "/app/dashboard", icon: <DashboardIcon /> },

  { text: "Courts", path: "/app/courts", icon: <AccountBalanceIcon /> },

  { text: "Cases", path: "/app/cases", icon: <FolderIcon /> },

  { text: "Reports", path: "/app/reports", icon: <AssessmentIcon /> },

  { text: "Documents", path: "/app/documents", icon: <DescriptionIcon /> },

  { text: "Website", path: "/app/website", icon: <LanguageIcon />, adminOnly: true },

  { text: "Accounts", path: "/app/accounts", icon: <PaymentsIcon />, adminOnly: true },

];



const superAdminItems: MenuItem[] = [

  { text: "Organizations", path: "/app/organizations", icon: <BusinessIcon /> },

];



const AppLayout = () => {

  const [mobileOpen, setMobileOpen] = useState(false);

  const { logout, user, isAdministrator, isSuperAdmin } = useAuth();

  const navigate = useNavigate();

  const location = useLocation();



  const visibleItems = isSuperAdmin

    ? superAdminItems

    : menuItems.filter((item) => !item.adminOnly || isAdministrator);



  const drawer = (

    <Box>

      <Toolbar sx={{ gap: 1 }}>

        <GavelIcon sx={{ color: "#c9a227" }} />

        <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>

          Attorneys

        </Typography>

      </Toolbar>

      <List>

        {visibleItems.map((item) => (

          <ListItemButton

            key={item.path}

            component={RouterLink}

            to={item.path}

            selected={location.pathname === item.path || location.pathname.startsWith(`${item.path}/`)}

            onClick={() => setMobileOpen(false)}

            sx={{ minHeight: 48 }}

          >

            <ListItemIcon>{item.icon}</ListItemIcon>

            <ListItemText primary={item.text} />

          </ListItemButton>

        ))}

      </List>

    </Box>

  );



  return (

    <Box sx={{ display: "flex", width: "100%", minWidth: 0, minHeight: "100dvh", boxSizing: "border-box" }}>

      <CssBaseline />

      <AppBar

        position="fixed"

        sx={{ zIndex: (theme) => theme.zIndex.drawer + 1, bgcolor: "#0d1b2a" }}

      >

        <Toolbar sx={{ justifyContent: "space-between", gap: 1, minHeight: { xs: 56, sm: 64 } }}>

          <Box sx={{ display: "flex", alignItems: "center", minWidth: 0, flex: 1 }}>

            <IconButton

              color="inherit"

              edge="start"

              aria-label="Open navigation"

              onClick={() => setMobileOpen(!mobileOpen)}

              sx={{ mr: 1, display: { md: "none" }, flexShrink: 0 }}

            >

              <MenuIcon />

            </IconButton>

            <Typography variant="h6" sx={{ fontSize: { xs: "1rem", sm: "1.25rem" } }} noWrap>

              {user?.firmName ?? "Case Management"}

            </Typography>

          </Box>

          <Box sx={{ display: "flex", alignItems: "center", gap: { xs: 0.5, sm: 2 }, flexShrink: 0 }}>

            <Typography variant="body2" sx={{ display: { xs: "none", sm: "block" } }} noWrap>

              {user?.userName}

              {user?.firmCode ? ` · ${user.firmCode}` : user?.role ? ` · ${user.role}` : ""}

            </Typography>

            <Button color="inherit" onClick={() => { logout(); navigate("/"); }} sx={{ minWidth: { xs: 64, sm: 80 } }}>

              Logout

            </Button>

          </Box>

        </Toolbar>

      </AppBar>

      <Box component="nav" sx={{ width: { md: drawerWidth }, flexShrink: { md: 0 } }}>

        <Drawer

          variant="temporary"

          open={mobileOpen}

          onClose={() => setMobileOpen(false)}

          ModalProps={{ keepMounted: true }}

          sx={{

            display: { xs: "block", md: "none" },

            "& .MuiDrawer-paper": { width: drawerWidth, boxSizing: "border-box" },

          }}

        >

          {drawer}

        </Drawer>

        <Drawer

          variant="permanent"

          sx={{

            display: { xs: "none", md: "block" },

            "& .MuiDrawer-paper": { width: drawerWidth, boxSizing: "border-box" },

          }}

          open

        >

          {drawer}

        </Drawer>

      </Box>

      <Box

        component="main"

        sx={{

          flexGrow: 1,

          p: { xs: 1.5, sm: 2, md: 3 },

          width: { md: `calc(100% - ${drawerWidth}px)` },

          mt: { xs: 7, sm: 8 },

          maxWidth: "100%",

          overflowX: "hidden",

        }}

      >

        <Outlet />

      </Box>

    </Box>

  );

};



export default AppLayout;

