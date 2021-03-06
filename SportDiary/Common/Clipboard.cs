﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace SportDiary.Common
{
    /// <summary>
    /// Буфер обмена
    /// </summary>
    static class Clipboard
    {
        #region Methods
        /// <summary>
        /// Сохранение в буфер обмена
        /// </summary>
        /// <param name="obj">Объект, который надо поместить в буфер обмена</param>
        public static async void SerializeAsync(object obj)
        {
            if(obj == null)
            {
                return;
            }

            StorageFile clipboardFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("Clipboard.cb", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(await ApplicationData.Current.TemporaryFolder.CreateFileAsync("type.t", CreationCollisionOption.ReplaceExisting), obj.GetType().ToString());

            Stream stream = await clipboardFile.OpenStreamForWriteAsync();
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());

            using (stream)
            {
                xmlSerializer.Serialize(stream, obj);
            }
        }

        /// <summary>
        /// Достать обьект из буфера обмена
        /// </summary>
        /// <returns>Возвращает объект, который находится в буфере обмена</returns>
        public async static Task<object> DeserializeAsync()
        {
            try
            {
                Stream stream = await ApplicationData.Current.TemporaryFolder.OpenStreamForReadAsync("Clipboard.cb");

                Type type = Type.GetType(await FileIO.ReadTextAsync(await ApplicationData.Current.TemporaryFolder.GetFileAsync("type.t")));

                if (type == null)
                {
                    return null;
                }

                XmlSerializer xmlSerializer = new XmlSerializer(type);

                using (stream)
                {
                    return xmlSerializer.Deserialize(stream);
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch(InvalidOperationException)
            {
                return null;
            }
        }
        #endregion
    }
}
