﻿using System.Collections.Generic;
using System.Drawing;
using System.Net;

namespace Artco
{
    public class Sprite
    {
        private List<Bitmap> _tmp_img_list;

        public static List<List<Sprite>> sprites { get; } = new List<List<Sprite>>();
        public string name { get; set; }
        public string remote_path { get; }
        public bool is_remove_back { get; }
        public Bitmap user_tab_preview_img { get; }

        public Sprite() { }

        public Sprite(string name, string remote_path, bool is_remove_back, Bitmap user_tab_preview_img)
        {
            this.name = name;
            this.remote_path = remote_path;
            this.is_remove_back = is_remove_back;
            this.user_tab_preview_img = user_tab_preview_img;
        }

        public Sprite(Sprite sprite) : this(sprite.name, sprite.remote_path, sprite.is_remove_back, sprite.user_tab_preview_img) { }

        public void SetTmpImgList(List<Bitmap> bitmaps)
        {
            _tmp_img_list = bitmaps;
        }

        public List<Bitmap> GetTmpImgList()
        {
            return _tmp_img_list;
        }

        public Bitmap GetSpriteImage()
        {
            if (_tmp_img_list == null || _tmp_img_list.Count == 0) {
                return ImageUtility.GetNormalizedSizeImg(ImageUtility.GetImageFromURL(remote_path));
            } else {
                return ImageUtility.GetNormalizedSizeImg(_tmp_img_list[0]);
            }
        }

        public static void UpdateSpriteData()
        {
            sprites[0].RemoveRange(0, sprites[0].Count);

            string[] file_names = FileManager.GetFtpFolderItems(FileManager.ftp_root_dir + "sprites/" + Setting.user_name + "/");
            WebClient downloader = FileManager.GetHttpClient();
            for (int i = 0; i < file_names.Length; i++) {
                string name = file_names[i].Substring(0, file_names[i].Length - 4);
                string remote_path = FileManager.http_root_dir + "sprites/" + Setting.user_name + "/" + file_names[i];
                Sprite sprite = new Sprite(name, remote_path, true, ImageUtility.GetImageFromURL(remote_path, downloader));

                sprites[0].Add(sprite);
            }
            downloader.Dispose();
        }

        public static void AddSpriteData(string[] datas)
        {
            for (int i = 0; i < sprites.Count; i++)
                sprites[i].RemoveRange(0, sprites[i].Count);

            sprites.RemoveRange(0, sprites.Count);

            if (sprites.Count == 0)
                sprites.Add(new List<Sprite>());

            // 저장소 유저 이미지 미리 다운로드
            string[] file_names = FileManager.GetFtpFolderItems(FileManager.ftp_root_dir + "sprites/" + Setting.user_name + "/");
            WebClient downloader = FileManager.GetHttpClient();
            for (int i = 0; i < file_names.Length; i++) {
                string name = file_names[i].Substring(0, file_names[i].Length - 4);
                string remote_path = FileManager.http_root_dir + "sprites/" + Setting.user_name + "/" + file_names[i];
                Sprite sprite = new Sprite(name, remote_path, true, ImageUtility.GetImageFromURL(remote_path, downloader));

                sprites[0].Add(sprite);
            }
            downloader.Dispose();

            const int row_cnt = 4;
            for (int i = 0; i <= datas.Length - row_cnt; i += row_cnt) {
                string name = (Setting.language.Equals("Korean")) ? datas[i] : datas[i + 1];
                int category = int.Parse(datas[i + 2]);
                string remote_path = FileManager.http_root_dir + datas[i + 3];

                for (; category >= sprites.Count;)
                    sprites.Add(new List<Sprite>());

                sprites[category].Add(new Sprite(name, remote_path, false, null));
            }
        }

        public static Sprite GetNameToSprite(string name)
        {
            foreach (var sprite in sprites) {
                var ret = sprite.Find(item => item.name.Equals(name));
                if (ret != null)
                    return ret;
            }

            return null;
        }
    }
}