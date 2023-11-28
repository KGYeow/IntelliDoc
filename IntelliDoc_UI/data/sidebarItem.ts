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
}

const sidebarItem: menu[] = [
  { header: 'Home' },
  {
    title: 'Dashboard',
    icon: tablerIcon.LayoutDashboardIcon,
    to: '/dashboard',
    auth: true
  },
  {
    title: 'Document',
    icon: tablerIcon.FileDescriptionIcon,
    children: [
      {
        title: 'Repository',
        icon: tablerIcon.BooksIcon,
        to: '/document/repository',
        auth: true
      },
      {
        title: 'Archive',
        icon: tablerIcon.ArchiveIcon,
        to: '/document/archive',
        auth: true
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
      },
    ]
  },
];

export default sidebarItem;