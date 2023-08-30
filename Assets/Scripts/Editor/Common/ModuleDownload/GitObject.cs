using System;
using System.Collections.Generic;
using System.Text;

namespace AKIRA.Editor.Git {
    /// <summary>
    /// Git Json Object
    /// </summary>
    [Serializable]
    public class GitObject {
        public Payload payload;
        public string title;
        public string locale;

        public Dictionary<string, GitObject> children = new Dictionary<string, GitObject>();

        public override string ToString() {
            return 
$@"object: {payload}
title: {title}
locale: {locale}";
        }
    }

    [Serializable]
    public class Payload {
        public bool allShortcutsEnabled;
        public string path;
        public Repo repo;
        public string currentUser;
        public RefInfo refInfo;
        public Tree tree;
        public FileTree fileTree;
        public float fileTreeProcessingTime;
        public string[] foldersToFetch;
        public bool showSurveyBanner;
        public bool showCodeNavSurvey;
        public Dictionary<string, string> csrf_tokens;

        public Blob blob;
    }

    [Serializable]
    public class Repo {
        public int id;
        public string defaultBranch;
        public string name;
        public string ownerLogin;
        public bool currentUserCanPush;
        public bool isFork;
        public bool isEmpty;
        public string createdAt;
        public string ownerAvatar;
        public bool isOrgOwned;
    }

    [Serializable]
    public class RefInfo {
        public string name;
        public string listCacheKey;
        public bool canEdit;
        public string refType;
        public string currentOid;
    }

    [Serializable]
    public class Tree {
        public TreeItem[] items;
        public string templateDirectorySuggestionUrl;
        public string readme;
        public int totalCount;
        public bool showBranchInfobar;

        public override string ToString() {
            string result = default;
            foreach (var item in items)
                result += item.ToString() + "\n";
            return result;
        }
    }

    [Serializable]
    public class TreeItem {
        public string name;
        public string path;
        public string contentType;

        public enum ItemType {
            Directory,
            File,
        }

        public ItemType ContentType {
            get {
                if (contentType == "directory")
                    return ItemType.Directory;
                else
                    return ItemType.File;
            }
        }

        public override string ToString() {
            return 
$@"节点名称：{name}
节点路径：{path}
节点类型：{contentType}";
        }
    }

    [Serializable]
    public class FileTree {
        public Dictionary<string, FileTreeItem> items;
        public int totalCount;
    }

    [Serializable]
    public class FileTreeItem {
        public string name;
        public string path;
        public string contentType;
    }

    [Serializable]
    public class Blob {
        public string[] rawLines;
        public object[] stylingDirectives;
        public object csv;
        public object csvError;
        public object dependabotInfo;
        public string displayName;
        public string displayUrl;
        public object headerInfo;
        public bool image;
        public object isCodeownersFile;
        public bool isValidLegacyIssueTemplate;
        public string issueTemplateHelpUrl;
        public object issueTemplate;
        public object discussionTemplate;
        public string language;
        public bool large;
        public bool loggedIn;
        public string newDiscussionPath;
        public string newIssuePath;
        public object planSupportInfo;
        public object publishBannersInfo;
        public bool renderImageOrRaw;
        public object richText;
        public object renderedFileInfo;
        public int tabSize;
        public object topBannersInfo;
        public bool truncated;
        public bool viewable;
        public object workflowRedirectUrl;
        public object symbols;
    }
}