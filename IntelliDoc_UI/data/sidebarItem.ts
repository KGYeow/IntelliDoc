import {
  CopyIcon, BooksIcon, ArchiveIcon,
  LayoutDashboardIcon, MoodHappyIcon, TypographyIcon
} from 'vue-tabler-icons';

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
}

const sidebarItem: menu[] = [
  { header: 'Home' },
  {
    title: 'Dashboard',
    icon: LayoutDashboardIcon,
    to: '/'
  },
  { header: 'Document' },
  {
    title: 'Repository',
    icon: BooksIcon,
    to: '/repository'
  },
  {
    title: 'Archive',
    icon: ArchiveIcon,
    to: '/archive'
  },
  { header: 'Extra' },
  {
    title: 'Typography',
    icon: TypographyIcon,
    to: '/typography'
  },
  {
    title: 'Shadow',
    icon: CopyIcon,
    to: '/shadow'
  },
  {
    title: 'Icons',
    icon: MoodHappyIcon,
    to: '/icons'
  },
];

export default sidebarItem;