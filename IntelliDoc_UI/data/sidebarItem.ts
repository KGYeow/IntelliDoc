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
        accessName: "DocumentRepository",
      },
      {
        title: 'Flagged',
        icon: tablerIcon.Flag3Icon,
        to: '/document/flag',
        auth: true,
        accessName: "DocumentFlag",
      },
      {
        title: 'Archive',
        icon: tablerIcon.ArchiveIcon,
        to: '/document/archive',
        auth: true,
        accessName: "DocumentArchive",
      },
    ]
  },
  {
    title: "Configuration",
    icon: tablerIcon.SettingsIcon,
    children: [
      {
        title: "User Setting",
        icon: tablerIcon.UserCogIcon,
        to: "/configuration/user-setting",
        auth: true,
        accessName: "ConfigurationUserSetting",
      },
      // {
      //   title: "User Manual",
      //   icon: tablerIcon.FileStackIcon,
      //   to: "/configuration/user-manual",
      //   auth: true,
      //   accessName: "ConfigurationUserManual",
      // },
    ]
  },
]

export default sidebarItem