import * as tablerIcon from "vue-tabler-icons";

export interface menu {
  header?: string;
  title?: string;
  icon?: any;
  to?: string;
  chip?: string;
  chipColor?: string;
  chipVariant?: string;
  chipIcon?: string;
  children?: menu[];
  disabled?: boolean;
  type?: string;
  subCaption?: string;
  auth?: boolean;
  accessName?: string;
}

const sidebarItem: menu[] = [
  { header: 'Home' },
  {
    title: 'Dashboard',
    icon: tablerIcon.LayoutDashboardIcon,
    to: '/dashboard',
    auth: true,
    accessName: "Dashboard",
  },
  {
    title: 'Document',
    icon: tablerIcon.FileDescriptionIcon,
    children: [
      {
        title: 'Repository',
        icon: tablerIcon.BooksIcon,
        to: '/document/repository',
        auth: true,
        accessName: "Repositories",
      },
      {
        title: 'Archive',
        icon: tablerIcon.ArchiveIcon,
        to: '/document/archive',
        auth: true,
        accessName: "Archives",
      },
    ]
  },
  {
    title: "Configuration",
    icon: tablerIcon.SettingsIcon,
    children: [
      {
        title: "User Settings",
        icon: tablerIcon.UserCogIcon,
        to: "/configuration/user-settings",
        auth: true,
        accessName: "UserSettings",
      },
    ]
  },
]

export default sidebarItem