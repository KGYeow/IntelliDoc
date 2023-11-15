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
  { header: 'Document' },
  {
    title: 'Repository',
    icon: tablerIcon.BooksIcon,
    to: '/document/repository',
  },
  {
    title: 'Archive',
    icon: tablerIcon.ArchiveIcon,
    to: '/document/archive',
    auth: true
  },
  { header: 'Configuration' },
  {
    title: 'User Management',
    icon: tablerIcon.UserCogIcon,
    to: '/configuration/userManagement',
    auth: true
  },
  { header: 'Sample Page' },
  {
    title: 'Sample Page',
    icon: tablerIcon.ApertureIcon,
    to: '/sample-page',
    auth: true
  },
  {
    title: 'Typography',
    icon: tablerIcon.TypographyIcon,
    to: '/sample-page/typography'
  },
  {
    title: 'Shadow',
    icon: tablerIcon.CopyIcon,
    to: '/sample-page/shadow'
  },
  {
    title: 'Icons',
    icon: tablerIcon.MoodHappyIcon,
    to: '/sample-page/icons'
  },
];

export default sidebarItem;